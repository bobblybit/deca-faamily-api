using NotificationService.EmailModel;
using System.Threading.Tasks;

namespace NotificationService.Interface
{
    public interface IEmailNotification
    {
        Task<bool> SendEmailAsync(Email email);
        Task<bool> SendEmailAsync(string recipientEmail, string subject, string htmlContent, string plainText = "");
    }
}
