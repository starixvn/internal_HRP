namespace HRP.Core.Services
{
    using System.IO;
    using System.Net;
    using System.Net.Mail;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Model;

    public class BaseMailService : IBaseMailService
    {
        private readonly IOptionsSnapshot<MailConfigs> configuration;

        public BaseMailService(IOptionsSnapshot<MailConfigs> configuration)
        {
            this.configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(MailModel email)
        {
            bool mailSent = false;
            if (email == null || !IsValidEmail(email.ReceiverEmail))
            {
                return mailSent;
            }

            using (var message = new MailMessage())
            {
                message.From = new MailAddress(configuration.Value.Username);
                message.To.Add(new MailAddress(email.ReceiverEmail));
                message.Subject = email.Subject;
                message.Body = email.Body;
                message.IsBodyHtml = configuration.Value.IsBodyHtml;

                foreach (Attachment attachment in email.Attachments)
                {
                    message.Attachments.Add(attachment);
                }

                using (var mailclient = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = configuration.Value.Username,
                        Password = configuration.Value.Password
                    };
                    mailclient.UseDefaultCredentials = true;
                    mailclient.Credentials = credential;
                    mailclient.Host = configuration.Value.SmtpServer;
                    mailclient.Port = configuration.Value.SmtpPortNumber;
                    mailclient.EnableSsl = configuration.Value.EnableSsl;
                    mailclient.SendCompleted += (s, e) =>
                    {
                        mailSent = !(e.Cancelled || e.Error != null);
                    };

                    await mailclient.SendMailAsync(message).ConfigureAwait(false);
                }
            }

            return mailSent;
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