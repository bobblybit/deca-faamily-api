using ArticleService.Data.Dtos;
using ArticleService.Data.Models;

using AutoMapper;
using System.Collections.Generic;

namespace ArticleService.MappingProfiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CategoryToAddDto, Category>().ReverseMap();
            CreateMap<Category, CategoryToReturnDto>().ReverseMap();
            CreateMap<Category, CategoryToUpdateDto>().ReverseMap();
        }
    }
}
