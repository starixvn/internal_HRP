namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetCostCenterBySectionIdCriteria : CriteriaBase
    {
        public int SectionId { get; set; }

        public override bool IsValid() => SectionId > 0;

        public override string GetSettingKey() => "CostCenter_GetBySectionId";
    }
}