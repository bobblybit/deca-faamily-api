using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MqDtos
{
    public class Error
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public string DateCreated { get; set; }
    }
}
