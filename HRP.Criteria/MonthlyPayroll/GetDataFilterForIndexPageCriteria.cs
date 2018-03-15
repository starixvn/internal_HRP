namespace HRP.Criteria.MonthlyPayroll
{
    using System;
    using Fanex.Data.Repository;

    public class GetDataFilterForIndexPageCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public DateTime Month { get; set; }

        public int CompanyId { get; set; }

        public string CostCenterIds { get; set; }

        public override string GetSettingKey() => "MonthlyPayroll_GetDataFilterForIndexPage";

        public override bool IsValid() => UserId > 0 && CompanyId > 0;
    }
}