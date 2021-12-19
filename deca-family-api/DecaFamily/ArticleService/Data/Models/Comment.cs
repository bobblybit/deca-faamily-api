using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticleService.Data.Models
{
    public class Comment: BaseEntity
    {
        public string ArticleId { get; set; }
        public Article Article { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string Content { get; set; }
        public ICollection<CommentLike> CommentLikes { get; set; }

        public Comment()
        {
            CommentLikes = new HashSet<CommentLike>();
        }
    }
}
