using AccountManagement.Data.Dtos;
using AccountManagement.Data.Models;

using AutoMapper;

namespace AccountManagement.MappingProfiles
{
    public class StackAndSquadProfile: Profile
    {
        public StackAndSquadProfile()
        {
            CreateMap<StackToAddDto, MyStack>();
            CreateMap<SquadToAddDto, MySquad>();
            CreateMap<StackToUpdateDto, MyStack>();
            CreateMap<SquadToUpdateDto, MySquad>();
            CreateMap<MyStack, StackToReturnDto>().ReverseMap();
            CreateMap<MySquad, SquadToReturnDto>().ReverseMap();
        }
    }
}
