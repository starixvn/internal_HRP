namespace HRP.Core.Services
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Mail;
    using System.Text.RegularExpressions;
    using HRP.Core.Criteria;
    using HRP.Core.Model;

    public class BaseMailService : IBaseMailService
    {
        private readonly IBaseService baseService;

        public BaseMailService(IBaseService baseService)
        {
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
                message.From = new MailAddress(email.Username);
                message.To.Add(new MailAddress(email.ReceiverEmail));
                message.Subject = email.Subject;
                message.Body = email.Body;
                message.IsBodyHtml = true;
                message.Priority = email.Priority;

                foreach (Attachment attachment in email.Attachments)
                {
                    message.Attachments.Add(attachment);
                }

                using (var mailclient = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = email.Username,
                        Password = email.Password
                    };
                    mailclient.UseDefaultCredentials = true;
                    mailclient.Credentials = credential;
                    mailclient.Host = email.Server;
                    mailclient.Port = email.Port;
                    mailclient.EnableSsl = email.IsAuth;

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