namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class CheckCompanyExistsCriteria : CriteriaBase
    {
        public int CompanyId { get; set; }

        public string Code { get; set; }

        public override bool IsValid() => CompanyId >= 0 && !string.IsNullOrWhiteSpace(Code);

        public override string GetSettingKey() => "Company_CheckExists";
    }
}