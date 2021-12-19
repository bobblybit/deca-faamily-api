using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using MqDtos;
using NotificationService.Helper;
using NotificationService.Interface;
using MqDtos;

namespace NotificationService.ConsumerClasses
{
    public class ResetPasswordConsumer : IConsumer<ResetPasswordMqDto>
    {
        private readonly ILogger<NewUserConsumer> _logger;
        private readonly IEmailNotification _emailService;

        public ResetPasswordConsumer(IEmailNotification emailService, ILogger<NewUserConsumer> logger)
        {
            _logger = logger;
            _emailService = emailService;
        }
        public async Task Consume(ConsumeContext<ResetPasswordMqDto> context)
        {
            try
            {
                //TODO write your code here

                var template = EmailTemplateHelper.CreateTemplate($"{context.Message.FirstName} {context.Message.LastName}",
                    context.Message.BaseAddress, context.Message.Link, "resetpasswordemailtemplate.html");

                await _emailService.SendEmailAsync(context.Message.Email, "Reset Password", template, "");

            }
            catch (Exception e)
            {
                _logger.LogError($"Error message: {e.Message}");
            }

            _logger.LogInformation("Sends reset password email from queue to user!");
        }
    }
}
