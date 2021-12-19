using ArticleService.Data.Dtos;
using ArticleService.Data.Models;
using AutoMapper;

namespace ArticleService.MappingProfiles
{
    public class CommentProfile:Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentResponseDto>().ReverseMap();
            CreateMap<CommentToAddDto, Comment>().ReverseMap();
            CreateMap<Comment, CommentToReturnDto>().ReverseMap();
        }
    }
}