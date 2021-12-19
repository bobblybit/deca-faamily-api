using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticleService.Data.Models
{
    public class CommentLike: BaseEntity
    {
        public string CommentId { get; set; }
        public Comment Comment { get; set; }
        public string Liker { get; set; }
        public string LikerId { get; set; }

    }
}
