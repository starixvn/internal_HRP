namespace HRP.Criteria.Company
{
    public class CreateCompanyCriteria : CommandBaseCriteria
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public override bool IsValid() => !string.IsNullOrEmpty(Code);

        public override string GetSettingKey() => "Company_Create";
    }
}