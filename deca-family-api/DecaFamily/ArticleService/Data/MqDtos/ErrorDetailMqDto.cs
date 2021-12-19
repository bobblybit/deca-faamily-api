using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MqDtos
{
    public class ErrorDetailMqDto
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Source { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StackTrace { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(new ErrorDetailMqDto { StatusCode = this.StatusCode, Message = this.Message });
        }
    }
}
