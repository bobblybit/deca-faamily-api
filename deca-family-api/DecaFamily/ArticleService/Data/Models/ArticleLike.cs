using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticleService.Data.Models
{
    public class ArticleLike : BaseEntity
    {
        public string ArticleId { get; set; }
        public Article Article { get; set; }
        public string Liker { get; set; }
        public string LikerId { get; set; }
    }
}
