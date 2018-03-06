namespace HRP.Criteria.MonthlyPayroll
{
    using System;
    using Fanex.Data.Repository;

    public class GetMonthlyPayrollCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public int CompanyId { get; set; }

        public DateTime Month { get; set; }

        public string CostCenterIds { get; set; }

        public int? EmployeeId { get; set; }

        public int? WorkingDays { get; set; }

        public override string GetSettingKey() => "MonthlyPayroll_Get";

        public override bool IsValid() => UserId > 0 && CompanyId > 0;
    }
}