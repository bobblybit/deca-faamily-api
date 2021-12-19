using MassTransit;
using Microsoft.Extensions.Options;
using MqDtos;
using NotificationService.Helper;
using NotificationService.Interface;
using NotificationService.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.ConsumerClasses
{
    public class ExceptionListConsumer : IConsumer<ExceptionListMqDto>
    {
        private readonly IEmailNotification _emailService;
        private EmailSenderConfiguration Configuration { get; set; }

        public ExceptionListConsumer(IEmailNotification emailService, IOptions<EmailSenderConfiguration> emailConfig)
        {
            _emailService = emailService;
            Configuration = emailConfig.Value;
        }

        public async Task Consume(ConsumeContext<ExceptionListMqDto> context)
        {
            try
            {
                //TODO Create template and send email

                var template = EmailTemplateHelper.CreateTemplate(context.Message, "ErrorListTemplate.html");

                await _emailService.SendEmailAsync(Configuration.ErrorReceiverEmail, "Exception List", template, "");

            }
            catch (Exception e)
            {
               //TODO: What to do with exception in notification service?
            }

        }
    }
}
