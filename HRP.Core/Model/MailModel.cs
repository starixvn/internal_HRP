namespace HRP.Core.Model
{
    using System.Collections.Generic;
    using System.Net.Mail;

    public class MailModel
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAuth { get; set; }
        public MailPriority Priority { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string ReceiverEmail { get; set; }

        public List<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}