using AccountManagement.Data.Models;
using System.Collections.Generic;

namespace AccountManagement.Data.Dtos
{
    public class PaginatedResultDto<T>
    {
        public PageMetaData PageMetaData { get; set; }
        public IEnumerable<T> ResponseData { get; set; } = new List<T>();
    }
}