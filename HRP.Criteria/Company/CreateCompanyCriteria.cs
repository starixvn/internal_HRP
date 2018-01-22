namespace HRP.Criteria.Company
{
    using Fanex.Data.Repository;

    public class CreateCompanyCriteria : CriteriaBase
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public override bool IsValid() => !string.IsNullOrEmpty(Code);

        public override string GetSettingKey() => "DataConfiguration_CreateCompanyBVI";
    }
}