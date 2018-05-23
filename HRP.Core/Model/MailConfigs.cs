namespace HRP.Core.Model
{
    public class MailConfigs
    {
        public string SmtpServer { get; set; }
        public int SmtpPortNumber { get; set; }
        public bool EnableSsl { get; set; }
        public bool IsBodyHtml { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}