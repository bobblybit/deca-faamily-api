using AccountManagement.Data.Dtos;
using AccountManagement.Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountManagement.Data.Repositories.Interfaces
{
    public interface IPhotoRepository
    {
        Task<UploadAvatarResponseDto> UploadAvatarAsync(IFormFile file);
        bool DeleteAvatar(string publicId);
        Task<IEnumerable<Photo>> GetRecentPhotosAsync(int page = 9);
        Task<bool> DeletePhotoAsync(string publicId);
    }
}