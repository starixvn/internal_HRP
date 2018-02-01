namespace HRP.Criteria.Department
{
    public class UpdateDepartmentCriteria : CommandBaseCriteria
    {
        public int DepartmentId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int UpdatedBy { get; set; }

        public override string GetSettingKey() => "Department_Update";

        public override bool IsValid() => DepartmentId > 0 && !string.IsNullOrWhiteSpace(Code) && UpdatedBy > 0;
    }
}