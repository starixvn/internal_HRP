namespace HRP.Criteria.CostCenter
{
    using Fanex.Data.Repository;

    public class GetCostCenterByIdCriteria : CriteriaBase
    {
        public int CostCenterId { get; set; }

        public override string GetSettingKey() => "CostCenter_GetById";

        public override bool IsValid() => CostCenterId > 0;
    }
}