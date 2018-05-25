namespace HRP.Core.Services
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Mail;
    using System.Text.RegularExpressions;
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

        public bool SendEmail(MailModel email)
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

                    try
                    {
                        mailclient.Send(message);

                        mailSent = true;
                        WriteLogSendEmail(email, mailSent);
                    }
                    catch (SmtpException ex)
                    {
                        WriteLogSendEmail(email, mailSent, ex.Message, ex.StackTrace);
                    }
                    catch (Exception ex)
                    {
                        WriteLogSendEmail(email, mailSent, ex.Message, ex.StackTrace);
                    }

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

        private void WriteLogSendEmail(
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

            baseService.ExecuteNonQueryAsync(criteria).Wait();
        }
    }
}