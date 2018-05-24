namespace HRP.Core.Criteria
{
    using Fanex.Data.Repository;

    public class LogMailErrorCriteria : CriteriaBase
    {
        public string MailTo { get; set; }
        public string Subject { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "MailError_Create";
    }
}