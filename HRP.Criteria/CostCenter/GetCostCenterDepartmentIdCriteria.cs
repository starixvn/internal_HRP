namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetCostCenterDepartmentIdCriteria : CriteriaBase
    {
        public int DepartmentId { get; set; }

        public bool? IsActive { get; set; }

        public override bool IsValid() => DepartmentId > 0;

        public override string GetSettingKey() => "CostCenter_GetByDepartmentId";
    }
}