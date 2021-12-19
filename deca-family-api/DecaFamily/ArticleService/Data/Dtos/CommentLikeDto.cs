using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticleService.Data.Dtos
{
    public class CommentLikeDto
    {
        public string Id { get; set; }
        public string CommentId { get; set; }
        public string LikerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}