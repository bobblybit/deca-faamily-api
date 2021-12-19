using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationService.EmailModel;
using NotificationService.Interface;
using NotificationService.Settings;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.Implementation
{
    public class EmailNotification : IEmailNotification
    {

        public ILogger<EmailNotification> Logger { get; }

        private EmailSenderConfiguration Configuration { get; set; }

        public EmailNotification(IOptions<EmailSenderConfiguration> emailConfig, ILogger<EmailNotification> logger)
        {
            Configuration = emailConfig.Value;
        }

        public async Task<bool> SendEmailAsync(Email email)
        {
            try
            {
                var client = new SendGridClient(Configuration.ApiKey);
                var message = new SendGridMessage()
                {
                    From = new EmailAddress(Configuration.SenderEmail, Configuration.SenderName),
                    Subject = email.Subject,
                    PlainTextContent = email.PlainMessage,
                    HtmlContent = email.Body,
                };
                var recipients = new List<EmailAddress>();
                foreach (var recipient in email.Recipients)
                {
                    recipients.Add(new EmailAddress(recipient));
                }

                message.AddTos(recipients);


                message.SetOpenTracking(false);
                message.SetClickTracking(false, false);
                message.SetSubscriptionTracking(false);
                message.SetGoogleAnalytics(false);

                var response = await client.SendEmailAsync(message).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                    return true;

            }
            catch (Exception ex)
            {
                return false;
            }

            return false;
        }

        public async Task<bool> SendEmailAsync(string recipientEmail, string subject, string htmlContent, string plainText = "")
        {
            try
            {
                var client = new SendGridClient(Configuration.ApiKey);
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress(Configuration.SenderEmail, Configuration.SenderName),
                    Subject = subject,
                    PlainTextContent = plainText,
                    HtmlContent = htmlContent
                };
                msg.AddTo(new EmailAddress(recipientEmail));

                //disable tracking by sendgrid
                msg.SetOpenTracking(false);
                msg.SetClickTracking(false, false);
                msg.SetSubscriptionTracking(false);

                //disable google analytics
                msg.SetGoogleAnalytics(false);
                var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.Accepted) return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, new string[] { recipientEmail });
                return false;
            }
            return false;
        }
    }
}
