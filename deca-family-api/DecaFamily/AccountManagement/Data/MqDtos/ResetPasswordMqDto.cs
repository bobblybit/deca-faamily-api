using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MqDtos
{
    public class ResetPasswordMqDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string BaseAddress { get; set; }
        public string Link { get; set; }
    }
}
