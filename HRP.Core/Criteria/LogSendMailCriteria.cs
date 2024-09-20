namespace HRP.Core.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class LogSendMailCriteria : NonQueryCommand
    {
        public string MailTo { get; set; }
        public string Subject { get; set; }
        public DateTime SentTime { get; set; }
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "MailRecord_Create";
    }
}