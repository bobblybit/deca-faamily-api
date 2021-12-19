
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using AccountManagement.Data.Dtos;
using AccountManagement.Data.Models;

using AutoMapper;

namespace AccountManagement.MappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<AppUser, UserToAddDto>().ReverseMap();
            CreateMap<AppUserAddress, AddressDto>().ReverseMap();
            CreateMap<UserToUpdateDto, AppUser>()
                .AfterMap(MapSocialHandles);
            CreateMap<Department, DepartmentToUpdateDto>().ReverseMap();
            CreateMap<PhotoToUpdateDto, Photo>().ReverseMap();
            CreateMap<SocialHandleToUpdateDto, SocialHandles>().ReverseMap();
        }

        public void MapSocialHandles(UserToUpdateDto data, AppUser user)
        {
            if (data.SocialHandles == null || data.SocialHandles.Count() < 1)
                return;

            if (user.SocialHandles == null || user.SocialHandles.Count() < 1)
            {
                var handlesToAdd = new List<SocialHandles>();
                foreach(var handle in data.SocialHandles)
                {
                    handlesToAdd.Add(Mapper.Map<SocialHandles>(handle));
                }
                user.SocialHandles = handlesToAdd;
                return;
            }

            foreach(var handle in user.SocialHandles)
            {
                var chechHandle = data.SocialHandles.SingleOrDefault(x => x.Name.ToLower() == handle.Name.ToLower());
                if(chechHandle != null)
                {
                    Mapper.Map(chechHandle, handle);
                }
            }
            foreach(var handle in data.SocialHandles)
            {
                var checkHandle = user.SocialHandles.SingleOrDefault(x => x.Name == handle.Name);
                if(checkHandle == null)
                {
                    user.SocialHandles.Add(Mapper.Map<SocialHandles>(handle));
                }
            }
        }
    }
}
