using MassTransit;
using Microsoft.Extensions.Logging;
using MqDtos;
using NotificationService.Helper;
using NotificationService.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.ConsumerClasses
{
    public class RoleNotificationConsumer : IConsumer<RoleNotificationDto>
    {
        private readonly ILogger<RoleNotificationDto> _logger;
        private readonly IEmailNotification _emailNotification;

        public RoleNotificationConsumer( ILogger<RoleNotificationDto> logger , IEmailNotification emailNotification)
        {
            _logger = logger;
            _emailNotification = emailNotification;
        }


        public async Task Consume(ConsumeContext<RoleNotificationDto> context)
        {

            try
            {
                string[] data = { $"{context.Message.FirstName} {context.Message.LastName}", context.Message.Role, context.Message.EmailTemplateName };
                var Template = EmailTemplateHelper.CreateTemplate(data);
                await _emailNotification.SendEmailAsync(context.Message.Email, "Role Notification", Template, "");
            }
            catch (Exception e)
            {

                _logger.LogError($"Error Message {e.Message}");
            }
            _logger.LogInformation("Added role from que to role");
        }
    }
}
