namespace HRP.Criteria.Department
{
    public class CreateDepartmentCriteria : CommandBaseCriteria
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int DivisionId { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "Department_Create";

        public override bool IsValid() => DivisionId > 0 && !string.IsNullOrWhiteSpace(Code) && CreatedBy > 0;
    }
}