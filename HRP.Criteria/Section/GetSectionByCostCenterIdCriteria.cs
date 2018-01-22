namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetSectionByCostCenterIdCriteria : CriteriaBase
    {
        public int CostCenterId { get; set; }

        public bool? IsActive { get; set; }

        public override bool IsValid() => CostCenterId > 0;

        public override string GetSettingKey() => "Section_GetByCostCenterId";
    }
}