namespace HRP.Criteria.Mail
{
    using Fanex.Data.Repository;

    public class GetMailSettingByCompanyIdCriteria : CriteriaBase
    {
        public int CompanyId { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "MailSetting_GetByCompanyId";
    }
}