namespace HRP.Criteria.CostCenter
{
    public class UpdateCostCenterCriteria : BaseNonQueryCommand
    {
        public int CostCenterId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int UpdatedBy { get; set; }

        public override string GetSettingKey() => "CostCenter_Update";

        public override bool IsValid() => CostCenterId > 0 && !string.IsNullOrWhiteSpace(Code) && UpdatedBy > 0;
    }
}