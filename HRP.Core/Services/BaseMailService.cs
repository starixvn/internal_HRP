namespace HRP.Core.Services
{
    using System.IO;
    using System.Text.RegularExpressions;
    using Fanex.MailDeliver.MailClient.API;
    using Fanex.MailDeliver.Model;
    using Microsoft.Extensions.Options;
    using Model;

    public class BaseMailService : HRPMailDeliver, IBaseMailService
    {
        public BaseMailService(IOptionsSnapshot<MailConfigs> configuration) : base(configuration)
        {
        }

        public bool SendEmail(MailModel email)
        {
            using (var client = GetEmailDeliver())
            {
                if (email == null || !IsValidEmail(email.ReceiverEmail))
                {
                    return false;
                }

                var mailItem = new MailRecord
                {
                    MailTo = email.ReceiverEmail,
                    MailCc = email.MailCC,
                    Subject = email.Subject,
                    Body = email.Body,
                    Attachments = email.Attachments
                };

                SendMail(mailItem);
            }

            return true;
        }

        public MailDeliver GetEmailDeliver()
        {
            return GetMailDeliver();
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