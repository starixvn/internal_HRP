namespace HRP.Criteria.CostCenter
{
    public class CreateCostCenterCriteria : BaseNonQueryCommand
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int DepartmentId { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "CostCenter_Create";

        public override bool IsValid() => DepartmentId > 0 && !string.IsNullOrWhiteSpace(Code) && CreatedBy > 0;
    }
}