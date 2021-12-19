using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArticleService.Data.Dtos
{
    public class AddArticleDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string  CategoryId { get; set; }
        [Required]
        public string Stack { get; set; }
        [Required]
        public string StackId { get; set; }
        public string  Tag { get; set; }
        [Required]
        public string Content { get; set; }
        [JsonIgnore]
        public bool Approved { get; set; } = false;
        public string ApprovedBy { get; set; } = null;
    }
}
