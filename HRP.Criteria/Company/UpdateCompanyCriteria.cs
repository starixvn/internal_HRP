namespace HRP.Criteria
{
    public class UpdateCompanyCriteria : CommandBaseCriteria
    {
        public int CompanyId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int UpdatedBy { get; set; }

        public override bool IsValid() => CompanyId > 0 && !string.IsNullOrWhiteSpace(Code) && UpdatedBy > 0;

        public override string GetSettingKey() => "Company_Update";
    }
}