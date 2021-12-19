using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AccountManagement.Data.Dtos;
using AccountManagement.Data.Models;

using AutoMapper;

namespace AccountManagement.MappingProfiles
{
    public class PhotoProfile: Profile
    {
        public PhotoProfile()
        {
            CreateMap<PhotoDto, Photo>().ReverseMap();
        }
    }
}
