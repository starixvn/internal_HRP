namespace HRP.Criteria.CostCenter
{
    using Fanex.Data.Repository;

    public class CheckCostCenterExistsCriteria : CriteriaBase
    {
        public int CostCenterId { get; set; }

        public int DepartmentId { get; set; }

        public string Code { get; set; }

        public override string GetSettingKey() => "CostCenter_CheckExists";

        public override bool IsValid() => CostCenterId >= 0 && DepartmentId >= 0 && !string.IsNullOrWhiteSpace(Code);
    }
}