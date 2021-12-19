using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticleService.Data.Dtos
{
    public class CategoriesToReturnDto
    {
        public IEnumerable<CategoryToReturnDto> CategoriesToReturn { get; set; } = new List<CategoryToReturnDto>();
    }
}