namespace HRP.Core.Model
{
    using System.Collections.Generic;
    using Fanex.MailDeliver.Model;

    public class MailModel
    {
        public string Subject { get; set; }

        public string Body { get; set; }

        public string ReceiverEmail { get; set; }

        public string MailCC { get; set; }

        public List<MailAttachment> Attachments { get; set; } = new List<MailAttachment>();
    }
}