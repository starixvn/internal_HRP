namespace HRP.Core.Model
{
    using System.Net.Mail;

    public class MailConfigs
    {
        public string SmtpServer { get; set; }
        public int SmtpPortNumber { get; set; }
        public bool EnableSsl { get; set; }
        public MailPriority MailPriority { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}