using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MassTransit;

using Microsoft.Extensions.Options;
using MqDtos;
using NotificationService.Helper;
using NotificationService.Interface;

using NotificationService.Settings;

namespace NotificationService.ConsumerClasses
{
    public class DatabaseBackupConsumer : IConsumer<BackupEmailDto>
    {
        
        private readonly IEmailNotification _emailService;
        private EmailSenderConfiguration Configuration { get; set; }

        public DatabaseBackupConsumer(IEmailNotification emailService, IOptions<EmailSenderConfiguration> emailConfig)
        {
            _emailService = emailService;
            Configuration = emailConfig.Value;
        }

        public async Task Consume(ConsumeContext<BackupEmailDto> context)
        {
            try
            {
                //TODO Create template and send email

                var template = EmailTemplateHelper.CreateTemplate(context.Message.Date.ToString("f"), context.Message.Link, "DatabaseBackupNotificationTemplate.html");

                await _emailService.SendEmailAsync(Configuration.ErrorReceiverEmail, "Database Backup", template, "");

            }
            catch (Exception e)
            {
                //TODO: What to do with exception in notification service?
            }

        }
    }
}
