using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ArticleService.Data.Models
{
    public class Article: BaseEntity
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public string CategoryId { get; set; }
        public Category Category { get; set; }
        public string Stack { get; set; }
        public string StackId { get; set; }
        public string Tag { get; set; }
        public bool Approved { get; set; }
        public string ApprovedBy { get; set; }
        public ICollection<ArticleLike> ArticleLikes { get; set; }
        public ICollection<Comment> Comments { get; set; }

        public Article()
        {
            ArticleLikes = new HashSet<ArticleLike>();
            Comments = new HashSet<Comment>();
        }
    }
}
