using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MqDtos
{
    public class RoleNotificationDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string EmailTemplateName { get; set; }

    }
}
