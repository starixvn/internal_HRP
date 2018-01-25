namespace HRP.Criteria.Company
{
    using Fanex.Data.Repository;

    public class CheckCompanyExistsCriteria : CriteriaBase
    {
        public string Code { get; set; }

        public override bool IsValid() => !string.IsNullOrEmpty(Code);

        public override string GetSettingKey() => "DataConfiguration_CheckCompanyBVIExists";
    }
}