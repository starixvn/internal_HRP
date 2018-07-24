namespace HRP.Criteria.Mail
{
    using Fanex.Data.Repository;

    public class GetMailSettingCriteria : CriteriaBase
    {
        public override bool IsValid() => true;

        public override string GetSettingKey() => "MailSetting_Get";
    }
}