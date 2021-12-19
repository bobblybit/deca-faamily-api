using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticleService.Data.Dtos
{
    public class CommentToAddDto
    {
        public string ArticleId { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
    }
}