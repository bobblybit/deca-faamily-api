using ArticleService.Data.Dtos;
using ArticleService.Data.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticleService.MappingProfiles
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<AddArticleDto, Article>().ReverseMap();
            CreateMap<UpdateArticleDto, Article>().ReverseMap();
            CreateMap<Article, ArticleToReturnDto>().ReverseMap();
            CreateMap<Article, ArticleCardDto>().ForMember(x => x.AuthorId,
                opt => opt.MapFrom(src => src.UserId))
                .ForMember(x => x.Author, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(l => l.LikesCount,
                opt => opt.MapFrom(src => src.ArticleLikes.Count()))
                .ForMember(m => m.CommentCount, 
                opt => opt.MapFrom(src => src.Comments.Count()));
        }
    }
}
