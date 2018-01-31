namespace HRP.Criteria.CostCenter
{
    using Fanex.Data.Repository;

    public class GetAllCostCenterCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "CostCenter_GetAll";

        public override bool IsValid() => true;
    }
}