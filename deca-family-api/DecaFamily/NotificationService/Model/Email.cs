using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.EmailModel
{
    public class Email
    {
        public IEnumerable<string> Recipients { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string PlainMessage { get; set; } = "";
    }
}
