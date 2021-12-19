using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.Settings
{
    public class EmailSenderConfiguration
    {

        public const string ConfigSection = "EmailSenderConfiguration";
        public string ApiKey { get; set; }

        public string SenderName { get; set; }

        public string SenderEmail { get; set; }
        public string ErrorReceiverEmail { get; set; }
    }
}
