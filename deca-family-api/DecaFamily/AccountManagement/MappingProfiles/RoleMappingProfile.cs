using AccountManagement.Data.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountManagement.MappingProfiles
{
    public class RoleMappingProfile : Profile
    {

        public RoleMappingProfile()
        {
            CreateMap<IdentityRole, RoleToReturnDto>();
        }
    }
}
