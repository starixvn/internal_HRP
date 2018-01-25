namespace HRP.Core.Model
{
    public class MailModel
    {
        public string Subject { get; set; }

        public string Body { get; set; }

        public string ReceiverEmail { get; set; }

        public string MailCC { get; set; }
    }
}