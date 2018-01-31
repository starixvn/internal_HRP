namespace HRP.Criteria
{
    public class CreateCompanyCriteria : CommandBaseCriteria
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public override bool IsValid() => CreatedBy > 0 && !string.IsNullOrWhiteSpace(Code);

        public override string GetSettingKey() => "Company_Create";
    }
}