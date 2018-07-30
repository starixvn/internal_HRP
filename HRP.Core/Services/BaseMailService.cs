namespace HRP.Core.Services
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using HRP.Core.Criteria;
    using HRP.Core.Model;
    using MailKit.Net.Smtp;
    using MimeKit;

    public class BaseMailService : IBaseMailService
    {
        private readonly IBaseService baseService;

        public BaseMailService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<bool> SendEmailAsync(MailModel email)
        {
            bool mailSent = false;
            if (email == null || !IsValidEmail(email.ReceiverEmail))
            {
                return mailSent;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(email.Username));
            message.To.Add(new MailboxAddress(email.ReceiverEmail));
            message.Subject = email.Subject;
            message.Priority = (MessagePriority)email.Priority;

            var builder = new BodyBuilder
            {
                HtmlBody = email.Body
            };
            foreach (MimeEntity attachment in email.Attachments)
            {
                builder.Attachments.Add(attachment);
            }

            message.Body = builder.ToMessageBody();

            using (var mailclient = new SmtpClient())
            {
                await mailclient.ConnectAsync(email.Server, email.Port, email.IsAuth).ConfigureAwait(false);
                await mailclient.AuthenticateAsync(email.Username, email.Password).ConfigureAwait(false);

                try
                {
                    await mailclient.SendAsync(message).ConfigureAwait(false);

                    await mailclient.DisconnectAsync(true).ConfigureAwait(false);
                    mailSent = true;
                    await WriteLogSendEmail(email, mailSent).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await WriteLogSendEmail(email, mailSent, ex.Message, ex.StackTrace).ConfigureAwait(false);
                }

                return mailSent;
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

        private async Task WriteLogSendEmail(
            MailModel email,
            bool mailSent,
            string errorMessage = null,
            string stackTrace = null)
        {
            var criteria = new LogSendMailCriteria
            {
                MailTo = email.ReceiverEmail,
                Subject = email.Subject,
                SentTime = DateTime.Now,
                IsSuccessful = mailSent,
                ErrorMessage = errorMessage,
                StackTrace = stackTrace
            };

            await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }
    }
}