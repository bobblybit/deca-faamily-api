using AccountManagement.Data.Dtos;
using AccountManagement.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountManagement.Controllers.Helper
{
    public class Mapper
    {
        #region USER CONTROLLER HELPER MAPPER
        public static UserToReturnDto MapUser(AppUser userObj)
        {
            var userToReturn = new UserToReturnDto
            {
                UserId = userObj.Id,
                FirstName = userObj.FirstName,
                LastName = userObj.LastName,
                Gender = userObj.Gender,
                Email = userObj.Email,
                PhoneNumber = userObj.PhoneNumber,
                MyStack = new StackToReturnDto { Id = userObj.MyStack.Id, Name = userObj.MyStack.Name, Slugan = userObj.MyStack.Slugan },
                MySquad = new SquadToReturnDto { Id = userObj.MySquad.Id, Name = userObj.MySquad.Name, Slugan = userObj.MySquad.Slugan },
                Address = userObj.Address == null ? null : MapAddress(userObj.Address),
                Department = userObj.Department == null ? null : MapDepartment(userObj.Department),
                UserPhotos = userObj.Photos == null ? null : MapPhoto(userObj.Photos),
                SocialHandles = userObj.SocialHandles == null ? null : MapSocialHandles(userObj.SocialHandles)
            };
            return userToReturn;
        }

        public static UsersToReturnDto MapUsers(AppUser userObj)
        {
            var userToReturn = new UsersToReturnDto
            {
                UserId = userObj.Id,
                FirstName = userObj.FirstName,
                LastName = userObj.LastName,
                MyStack = userObj.MyStack == null ? "" : userObj.MyStack.Name,
                MySquad = userObj.MySquad == null ? "" : userObj.MySquad.Name,
                MainPhoto = userObj.Photos == null ? null : MapUserMainPhoto(userObj.Photos),
                SocialHandles = userObj.SocialHandles == null ? null : MapSocialHandles(userObj.SocialHandles)
            };
            return userToReturn;
        }

        private static AddressDto MapAddress(AppUserAddress userAddr)
        {
            var userAddress = new AddressDto
            {
                Street = userAddr.Street,
                City = userAddr.City,
                State = userAddr.State,
                Country = userAddr.Country
            };
            return userAddress;
        }

        private static DepartmentDto MapDepartment(Department userDept)
        {
            var comp = new CompanyDto
            {
                CompanyName = userDept.Company == null ? "" : userDept.Company.Name,
                CompanyLogo = userDept.Company == null ? "" : userDept.Company.Logo,
                CompanyMotto = userDept.Company == null ? "" : userDept.Company.Motto,
                CompanyMission = userDept.Company == null ? "" : userDept.Company.Mission,
                CompanyVision = userDept.Company == null ? "" : userDept.Company.Vision
            };

            var userDpt = new DepartmentDto
            {
                DepartmentName = userDept.Name,
                Position = userDept.Position,
                Company = comp
            };
            return userDpt;
        }

        private static List<PhotoDto> MapPhoto(ICollection<Photo> userPhotos)
        {
            var userPhotosToReturn = new List<PhotoDto>();

            foreach (var photo in userPhotos)
            {
                var photoToAdd = new PhotoDto
                {
                    PublicId = photo.PublicId,
                    PhotoUrl = photo.PhotoUrl,
                    IsMain = photo.IsMain,
                };
                userPhotosToReturn.Add(photoToAdd);
            }
            return userPhotosToReturn;
        }

        private static PhotoDto MapUserMainPhoto(ICollection<Photo> userPhotos)
        {
            var userMainPhoto = userPhotos.FirstOrDefault(p => p.IsMain == true);

            var photoToReturn = new PhotoDto
            {
                PublicId = userMainPhoto.PublicId,
                PhotoUrl = userMainPhoto.PhotoUrl,
                IsMain = userMainPhoto.IsMain
            };

            return photoToReturn;
        }

        public static IEnumerable<SquadToReturnDto> MapSquads(IEnumerable<MySquad> getAllSquads)
        {
            return getAllSquads.Select(x => new SquadToReturnDto { Id = x.Id, Name = x.Name }).ToList();
        }

        public static IEnumerable<StackToReturnDto> MapStacks(IEnumerable<MyStack> getAllStacks) 
        {
            return getAllStacks.Select(x => new StackToReturnDto { Id = x.Id, Name = x.Name, Slugan = x.Slugan }).ToList();
        }

        private static List<SocialHandleDto> MapSocialHandles(ICollection<SocialHandles> userSocialHandles)
        {
            var listOfSocialHandles = new List<SocialHandleDto>();

            foreach (var socialHandle in userSocialHandles)
            {
                var userAddress = new SocialHandleDto
                {
                    Name = socialHandle.Name,
                    Link = socialHandle.Link
                };

                listOfSocialHandles.Add(userAddress);
            }

            return listOfSocialHandles;
        }

        public static RoleToReturnDto MapRole(IdentityRole role)
        {
            var roleToReturn = new RoleToReturnDto { Id = role.Id, Name = role.Name };
            return roleToReturn;
        }

        public static IEnumerable<RecentPhotosDto> MapRecentPhotos(IEnumerable<Photo> usersPhotos)
        {
            return usersPhotos.Select(photos => new RecentPhotosDto
            {
                PhotoId = photos.Id,
                AppUserId = photos.AppUserId,
                PhotoUrl = photos.PhotoUrl,
                PublicId = photos.PublicId
            }).ToList();
        }
        #endregion

        #region STACK CONTROLLER HELPER MAPPER
        public static IEnumerable<RecentStackToReturnDto> MapGetRecentStacks(IEnumerable<MyStack> stacks)
        {
            return stacks.Select(stack => new RecentStackToReturnDto
            {
                Id = stack.Id,
                Name = stack.Name,
                Slogan = stack.Slugan,
                Members = stack.AppUsers.Count,
            }).ToList();
        }

        #endregion
    }
}
