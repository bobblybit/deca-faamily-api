using System.Collections.Generic;

namespace ArticleService.Data.Dtos
{
    public class CommentToReturnDto
    {
        public string ArticleId { get; set; }
        public string AuthorId { get; set; }
        public string Content { get; set; }
        public ICollection<CommentLikeDto> CommentLikes { get; set; } = new List<CommentLikeDto>();
    }
}