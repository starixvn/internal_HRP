namespace HRP.Core.Model
{
    using MimeKit;

    public class MailModel
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAuth { get; set; }
        public int Priority { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string ReceiverEmail { get; set; }
        public AttachmentCollection Attachments { get; set; } = new AttachmentCollection();
    }
}