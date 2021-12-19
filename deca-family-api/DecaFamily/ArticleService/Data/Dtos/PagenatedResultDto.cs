using ArticleService.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticleService.Data.Dtos
{
    public class PagenatedResultDto<T>
    {
      public PageMetaData PageMetaData { get; set; }
      public IEnumerable<T> ResponseData { get; set; } = new List<T>();
    }
}
