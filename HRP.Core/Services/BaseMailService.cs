namespace HRP.Core.Services
{
    using System.IO;
    using System.Text.RegularExpressions;
    using Fanex.MailDeliver.MailClient.API;
    using Fanex.MailDeliver.Model;
    using Microsoft.Extensions.Options;
    using Model;

    public class BaseMailService : IBaseMailService
    {
        private readonly IOptionsSnapshot<Model.MailConfig> _configuration;

        public BaseMailService(IOptionsSnapshot<Model.MailConfig> configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(MailModel email)
        {
            if (email == null || !IsValidEmail(email.ReceiverEmail))
            {
                return;
            }

            var mailItem = new MailRecord
            {
                MailTo = email.ReceiverEmail,
                MailCc = email.MailCC,
                Subject = email.Subject,
                Body = email.Body
            };

            using (var client = new MailDeliver(_configuration.Value.AppId, _configuration.Value.MailAddress))
            {
                client.SendMail(mailItem);
            }
        }

        public string ReadTextFile(string filePath)
        {
            using (var sr = new StreamReader(filePath))
            {
                return sr.ReadToEnd();
            }
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return regex.IsMatch(email);
        }
    }
}