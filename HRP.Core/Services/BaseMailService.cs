namespace HRP.Core.Services
{
    using System.ComponentModel;
    using System.IO;
    using System.Net;
    using System.Net.Mail;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using HRP.Core.Criteria;
    using Microsoft.Extensions.Options;
    using Model;

    public class BaseMailService : IBaseMailService
    {
        private readonly IOptionsSnapshot<MailConfigs> configuration;
        private readonly IBaseService baseService;

        public BaseMailService(
            IOptionsSnapshot<MailConfigs> configuration,
            IBaseService baseService)
        {
            this.configuration = configuration;
            this.baseService = baseService;
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
                message.IsBodyHtml = true;
                message.Priority = configuration.Value.MailPriority;

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

                        if (e.Cancelled || e.Error != null)
                        {
                            WriteLogErrorSendingEmail(email, e);
                        }
                    };

                    await mailclient.SendMailAsync(message).ConfigureAwait(false);

                    return mailSent;
                }
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

        private void WriteLogErrorSendingEmail(MailModel email, AsyncCompletedEventArgs e)
        {
            var criteria = new LogMailErrorCriteria
            {
                MailTo = email.ReceiverEmail,
                Subject = email.Subject
            };

            if (e.Cancelled)
            {
                criteria.ErrorMessage = "Send canceled.";
            }

            if (e.Error != null)
            {
                criteria.ErrorMessage = string.Format(
                    "ErrorMessage: {0} InnerException: {1}",
                    e.Error.Message,
                    e.Error.InnerException != null ? e.Error.InnerException.Message : string.Empty);
                criteria.StackTrace = e.Error.StackTrace;
            }

            baseService.ExecuteNonQueryAsync(criteria).Wait();
        }
    }
}