using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MqDtos;
using NotificationService.EmailModel;
using NotificationService.Helper;
using NotificationService.Interface;
using System;
using System.Threading.Tasks;

namespace NotificationService.ConsumerClasses
{
    public class NewUserConsumer : IConsumer<NewUserMqDto>
    {
        private readonly ILogger<NewUserConsumer> _logger;
        private readonly IEmailNotification _emailService;

        public NewUserConsumer(IEmailNotification emailService, ILogger<NewUserConsumer> logger)
        {
            _logger = logger;
            _emailService = emailService;
        }
        public async Task Consume(ConsumeContext<NewUserMqDto> context)
        {
            try
            {
                //TODO write your code here

                var template = EmailTemplateHelper.CreateTemplate($"{context.Message.FirstName} {context.Message.LastName}", 
                                                                     context.Message.BaseUrl, context.Message.Link, "EmailConfirmTemplate.html");

                await _emailService.SendEmailAsync(context.Message.Email, "Email Confirmation", template, "");

            }
            catch (Exception e)
            {
                _logger.LogError($"Error message: {e.Message}");
            }

            _logger.LogInformation("Added new user from queue to auth service!");
        }
    }
}
