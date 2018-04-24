namespace HRP.Criteria.MonthlyPayroll
{
    using Fanex.Data.Repository;

    public class GetHierarchyCompanyCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public int CompanyId { get; set; }

        public string CostCenterIds { get; set; }

        public override string GetSettingKey() => "MonthlyPayroll_GetHierarchyCompany";

        public override bool IsValid() => UserId > 0 && CompanyId > 0;
    }
}