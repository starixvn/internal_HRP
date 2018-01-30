namespace HRP.Criteria.Division
{
    public class CreateDivisionCriteria : CommandBaseCriteria
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int CompanyId { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "Division_Create";

        public override bool IsValid() => !string.IsNullOrEmpty(Code) && CompanyId > 0;
    }
}