using AccountManagement.Data.Database;
using AccountManagement.Data.Dtos;
using AccountManagement.Data.Models;
using AccountManagement.Data.Repositories.Interfaces;
using AccountManagement.Settings;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountManagement.Data.Repositories.Implementations
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly CloudinarySettings _options;
        private readonly Cloudinary _cloudinary;
        private readonly IConfiguration _configuration;
        private readonly DataContext _ctx;

        public PhotoRepository(IOptions<CloudinarySettings> options, IConfiguration configuration,
                                DataContext ctx)
        {
            _options = options.Value;
            Account account = new Account(
               _options.CloudName,
               _options.ApiKey,
               _options.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
            _configuration = configuration;
            _ctx = ctx;
        }

        public async Task<UploadAvatarResponseDto> UploadAvatarAsync(IFormFile file)
        {
            var pictureFormat = false;
            var listOfExtensions = _configuration.GetSection("PhotoSettings:Extensions").Get<List<string>>();
            for (int i = 0; i < listOfExtensions.Count; i++)
            {
                if (file.FileName.EndsWith(listOfExtensions[i]))
                {
                    pictureFormat = true;
                    break;
                }
            }

            if (pictureFormat == false)
                throw new Exception("File must be .jpg, .jpeg or .png");

            var pixSize = Convert.ToInt64(_configuration.GetSection("PhotoSettings:Size").Get<string>());
            if (file == null || file.Length > pixSize)
                throw new Exception("File size should not exceed 2mb");

            if (!pictureFormat)
                throw new Exception("File format is not supported. Please upload a picture");

            var imageUploadResult = new ImageUploadResult();
            using (var fs = file.OpenReadStream())
            {
                var imageUploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, fs),
                    Transformation = new Transformation().Width(300).Height(300)
                                                         .Crop("fill").Gravity("face")
                };
                imageUploadResult = await _cloudinary.UploadAsync(imageUploadParams);
            }

            UploadAvatarResponseDto result = new UploadAvatarResponseDto();

            if(imageUploadResult != null)
            {
                var publicId = imageUploadResult.PublicId;
                var photoUrl = imageUploadResult.Url.ToString();
                result = new UploadAvatarResponseDto
                {
                    PublicId = publicId,
                    PhotoUrl = photoUrl
                };
            }            

            return result;
        }

        public bool DeleteAvatar(string publicId)
        {
            var delParams = new DeletionParams(publicId) { ResourceType = ResourceType.Image };
            var result = _cloudinary.Destroy(delParams);

            if (result.Error != null)
                return false;

            return true;
        }

        public async Task<IEnumerable<Photo>> GetRecentPhotosAsync(int page)
        {
            return await _ctx.Photos.OrderByDescending(x => x.CreatedAt).Include(u => u.AppUser).Take(page).ToListAsync();
        }

        public async Task<bool> DeletePhotoAsync(string publicId)
        {
            var photo = await GetPhotoByPublicIdAsync(publicId);
            _ctx.Photos.Remove(photo);
            return await _ctx.SaveChangesAsync() > 0;
        }

        private async Task<Photo> GetPhotoByPublicIdAsync(string publicId)
        {
            return await _ctx.Photos.SingleOrDefaultAsync(x => x.PublicId == publicId);
        }
    }
}