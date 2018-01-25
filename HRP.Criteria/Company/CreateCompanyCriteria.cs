namespace HRP.Criteria.Company
{
    using Fanex.Data.Repository;

    public class CreateCompanyCriteria : CommandBaseCriteria
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public override bool IsValid() => !string.IsNullOrEmpty(Code);

        public override string GetSettingKey() => "DataConfiguration_CreateCompanyBVI";
    }
}