using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MqDtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Helper
{
    public class EmailTemplateHelper
    {
        public static string CreateTemplate(string fullName, string baseUrl, string link, string templateFilename)
        {
            //Read from the template file and construct the email template
            var templatePath = string.Join("\\", "..\\NotificationService\\wwwroot\\Template", templateFilename);
            var htmlContent = File.ReadAllText(templatePath);
            htmlContent = htmlContent.Replace("[name]", fullName);
            htmlContent = htmlContent.Replace("[baseAddress]", baseUrl);
            htmlContent = htmlContent.Replace("[link]", link);

            return htmlContent;
        }

        public static string CreateTemplate(string [] data)
        {
            var templatePath = string.Join("\\", "..\\NotificationService\\wwwroot\\Template", data[2]);
            var htmlContent = File.ReadAllText(templatePath);
            htmlContent = htmlContent.Replace("[name]" , data[0]);
            htmlContent = htmlContent.Replace("[role]", data[1]);

            return htmlContent;
        }

        public static string CreateTemplate(ExceptionListMqDto errorList, string templateFilename)
        {
            var errors = Build(errorList);
            //Read from the template file and construct the email template
            var templatePath = string.Join("\\", "..\\NotificationService\\wwwroot\\Template", templateFilename);
            var htmlContent = File.ReadAllText(templatePath);
            htmlContent = htmlContent.Replace("[errors]", errors);
            return htmlContent;
        }

        public static string CreateTemplate(string time, string link, string templateFilename)
        {
            //Read from the template file and construct the email template
            var templatePath = string.Join("\\", "..\\NotificationService\\wwwroot\\Template", templateFilename);
            var htmlContent = File.ReadAllText(templatePath);
            htmlContent = htmlContent.Replace("[backuptime]", time);
            htmlContent = htmlContent.Replace("[link]", link);
            return htmlContent;
        }

        public static string Build(ExceptionListMqDto errorList)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var error in errorList.Errors)
            {
                builder.Append("<code>");
                builder.Append("<b>");
                builder.Append($"MetaData: {error.DateCreated} {error.StatusCode} {error.Source}");
                builder.Append("</b>");
                builder.Append("<br/>");
                builder.Append($"Message: {error.Message}");
                builder.Append("<br/>");
                builder.Append($"{error.StackTrace}");
                builder.Append("</code>");
                builder.Append("<br/>");
                builder.Append("<br/>");
            }
            return builder.ToString();
        }
    }
}
