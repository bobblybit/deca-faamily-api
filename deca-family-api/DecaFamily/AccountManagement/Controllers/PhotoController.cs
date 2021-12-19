using AccountManagement.Data.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using AccountManagement.Data.Models;
using AccountManagement.Data.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AccountManagement.Commons;
using Microsoft.EntityFrameworkCore;

namespace AccountManagement.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class PhotoController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPhotoRepository _photoRepository;
        private readonly IMapper _mapper;

        public PhotoController(UserManager<AppUser> userManager, IPhotoRepository photoRepository, IMapper mapper)
        {
            _userManager = userManager;
            _photoRepository = photoRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Endpoint to add user's photo(s) using the service cloudinary
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("{userId}")]
        public async Task<IActionResult> AddPhotos(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser.Id != userId)
            {
                ModelState.AddModelError("User", "Cannot verify this User");
                return Unauthorized(Utilities.CreateResponse("Access denied", errs: ModelState, data: ""));
            }

            var user = await _userManager.Users
                                    .Include(x => x.Photos)
                                    .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                ModelState.AddModelError("User", "User does not exist");
                return NotFound(Utilities.CreateResponse("Invalid Credentials", errs: ModelState, data: ""));
            }

            var files = Request.Form.Files;
            if (user.Photos.Count() > 4 || (user.Photos.Count()+files.Count()) > 4)
            {
                ModelState.AddModelError("User photos", "User photos exceeds range of maximum photos allocated for a user");
                return BadRequest(Utilities.CreateResponse("Cannot upload more than four(4) photos", ModelState, ""));
            }

            var listOfPhotosToAdd = new List<Photo>();
            var result = new List<UploadAvatarResponseDto>();
            var errorList = new List<string>();

            foreach (var photo in files)
            {
                UploadAvatarResponseDto responseDto = new UploadAvatarResponseDto();
                try
                {
                    responseDto = await _photoRepository.UploadAvatarAsync(photo);

                    if (responseDto == null)
                    {
                        errorList.Add(photo.FileName);
                    }
                }
                catch (Exception e)
                {
                    errorList.Add(e.Message);
                }

                result.Add(responseDto);

                Photo photoToAdd = new Photo
                {
                    PhotoUrl = responseDto.PhotoUrl,
                    PublicId = responseDto.PublicId,
                    IsMain = false
                };

                listOfPhotosToAdd.Add(photoToAdd);
            }
            if(user.Photos != null)
            {
                foreach(var photo in listOfPhotosToAdd)
                {
                    user.Photos.Add(photo);
                }
            }
            else
            {
                user.Photos = listOfPhotosToAdd;
            }
            var updatedUser = await _userManager.UpdateAsync(user);

            if (!updatedUser.Succeeded)
            {
                foreach (var err in updatedUser.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }

                return BadRequest(Utilities.CreateResponse("Something went wrong", errs: ModelState, data: ""));
            }

            if (errorList.Count() > 0)
            {
                foreach (var error in errorList)
                {
                    ModelState.AddModelError("File could not be uploaded", error);
                }
                return BadRequest(Utilities.CreateResponse("Something went wrong", errs: ModelState, data: ""));
            }

            return Ok(Utilities.CreateResponse("Photo successfully edited!", errs: null, data: result));
        }

        /// <summary>
        /// Endpoint to delete user photo(s) using the cloudinary service
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeletePhotos(DeleteUserPhotosDto model, string userId)
        {
            var activeUser = await _userManager.GetUserAsync(User);
            
            if (activeUser.Id != userId)
            {
                ModelState.AddModelError("User", "Cannot verify this User");
                return Unauthorized(Utilities.CreateResponse("Access denied", errs: ModelState, data: ""));
            }

            var user = await _userManager.Users
                                    .Include(x => x.Photos)
                                    .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                ModelState.AddModelError("User", "User does not exist");
                return BadRequest(Utilities.CreateResponse("Invalid Credentials", errs: ModelState, data: ""));
            }

            var errs = new Dictionary<string, string>();

            foreach (var publicId in model.PublicIds)
            {
                Photo userPhotoObj = user.Photos.FirstOrDefault(x => x.PublicId == publicId);
                bool deletePhoto = await _photoRepository.DeletePhotoAsync(publicId);

                if (!deletePhoto)
                    errs.Add("Photo failed to delete", $"Failed to delete photo with {publicId} from database");

                bool deleteFromCloudinaryResult = _photoRepository.DeleteAvatar(publicId);

                if (!deleteFromCloudinaryResult)
                    errs.Add($"Delete failed {publicId}", $"Failed to delete from cloudinary for public id: {publicId}");
                var rs = await _userManager.UpdateAsync(user);
            }

            if (errs.Count > 0)
            {
                foreach (var err in errs)
                {
                    ModelState.AddModelError(err.Key, err.Value);
                }
                return BadRequest(Utilities.CreateResponse("Photo deletion error", ModelState, ""));
            }

            return Ok(Utilities.CreateResponse(message: "User photo(s) successfully deleted!", errs: null, data: ""));
        }

        /// <summary>
        /// Endpoint for getting all photos
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}/all-user-photos")]
        public async Task<IActionResult> GetAllPhotos(string userId) 
        {
            var user = await _userManager.Users
                                  .Include(x => x.Photos)
                                  .FirstOrDefaultAsync(x => x.Id == userId);
            if(user == null)
            {
                ModelState.AddModelError("Not Found", "User not found");
                return BadRequest(Utilities.CreateResponse<string>("Not Found", ModelState, ""));
            }

            var photos = _mapper.Map<PhotoDto[]>(user.Photos);
            return Ok(Utilities.CreateResponse<PhotoDto[]>("User Photos", null, photos));
        }

        /// <summary>
        /// Endpoint for setting user profile photo
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="photoId"></param>
        /// <returns></returns>
        [HttpPatch("{publicId}/user/{userId}/set-profile-photo")]
        public async Task<IActionResult> SetProfilePhoto(string userId, string publicId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            if (currentUser.Id != userId)
            {
                ModelState.AddModelError("User", "Cannot verify this User");
                return Unauthorized(Utilities.CreateResponse("Access denied", errs: ModelState, data: ""));
            }

            var user = await _userManager.Users
                           .Include(x => x.Photos)
                           .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                ModelState.AddModelError("Not Found", "User not found");
                return BadRequest(Utilities.CreateResponse("Not Found", ModelState, ""));
            }            

            foreach(var userPhoto in user.Photos)
            {
                if(userPhoto.IsMain)
                    userPhoto.IsMain = false;
            }

            var photo = user.Photos.FirstOrDefault(x => x.PublicId == publicId);
            if (photo == null)
            {
                ModelState.AddModelError("Not Found", "Photo not found");
                return BadRequest(Utilities.CreateResponse("Not Found", ModelState, ""));
            }

            photo.IsMain = true;
            await _userManager.UpdateAsync(user);
            var photoToReturn = _mapper.Map<PhotoDto>(photo);
            return Ok(Utilities.CreateResponse("Profile photo updated successfully", null, photoToReturn));
        }

        /// <summary>
        /// Endpoint for removing user profile photo
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="photoId"></param>
        /// <returns></returns>
        [HttpPatch("{publicId}/user/{userId}/unset-profile-photo")]
        public async Task<IActionResult> UnsetProfilePhoto(string userId, string publicId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser.Id != userId)
            {
                ModelState.AddModelError("User", "Cannot verify this User");
                return Unauthorized(Utilities.CreateResponse("Access denied", errs: ModelState, data: ""));
            }

            var user = await _userManager.Users
                           .Include(x => x.Photos)
                           .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                ModelState.AddModelError("Not Found", "User not found");
                return BadRequest(Utilities.CreateResponse<string>("Not Found", ModelState, ""));
            }

            var photo = user.Photos.FirstOrDefault(x => x.PublicId == publicId);
            if (photo == null)
            {
                ModelState.AddModelError("Not Found", "Photo not found");
                return BadRequest(Utilities.CreateResponse<string>("Not Found", ModelState, ""));
            }

            photo.IsMain = false;
            await _userManager.UpdateAsync(user);
            return Ok(Utilities.CreateResponse<string>("Profile photo removed successfuly", null, ""));
        }

        /// <summary>
        /// Endpoint for getting user photo
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="photoId"></param>
        /// <returns></returns>
        [HttpGet("{publicId}/user/{userId}/get-photo")]
        public async Task<IActionResult> GetPhoto(string userId, string publicId)
        {
            var user = await _userManager.Users
                           .Include(x => x.Photos)
                           .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                ModelState.AddModelError("Not Found", "User not found");
                return BadRequest(Utilities.CreateResponse<string>("Not Found", ModelState, ""));
            }

            var photo = user.Photos.FirstOrDefault(x => x.PublicId == publicId);
            if (photo == null)
            {
                ModelState.AddModelError("Not Found", "Photo not found");
                return BadRequest(Utilities.CreateResponse<string>("Not Found", ModelState, ""));
            }

            var photoToReturn = _mapper.Map<PhotoDto>(photo);
            return Ok(Utilities.CreateResponse("Profile photo removed successfuly", null, photoToReturn));
        }

        /// <summary>
        /// provides an endpoint for fetching recently uploaded photos of a few users to be displayed on the homepage
        /// </summary>
        [HttpGet("get-recent-photos")]
        [AllowAnonymous]
        public async Task<ActionResult> RecentPhotos()
        {
            var recentPhotos = await _photoRepository.GetRecentPhotosAsync();

            if (recentPhotos.Count() < 1)
                return BadRequest(Utilities.CreateResponse<string>("No recent user photos", ModelState, ""));

            var result = Helper.Mapper.MapRecentPhotos(recentPhotos);
            return Ok(Utilities.CreateResponse("Recent photos retrieved successfully", null, result));
        }
    }
}
