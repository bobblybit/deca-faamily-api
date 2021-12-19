using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MqDtos
{
    public class ExceptionListMqDto
    {
        public ExceptionListMqDto()
        {
            Errors = new List<Error>();
        }
        public List<Error> Errors { get; set; }
    }
}
