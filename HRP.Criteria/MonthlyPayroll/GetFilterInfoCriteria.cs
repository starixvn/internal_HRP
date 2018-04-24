namespace HRP.Criteria.MonthlyPayroll
{
    using System;
    using Fanex.Data.Repository;

    public class GetFilterInfoCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public int CompanyId { get; set; }

        public DateTime Month { get; set; }

        public int? PayrollId { get; set; }

        public override string GetSettingKey() => "MonthlyPayroll_GetFilterInfo";

        public override bool IsValid() => UserId > 0 && CompanyId > 0;
    }
}