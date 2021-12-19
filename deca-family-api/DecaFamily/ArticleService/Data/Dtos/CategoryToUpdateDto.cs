using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArticleService.Data.Dtos
{
    public class CategoryToUpdateDto
    {
        [Required]
        public string CategoryName { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
