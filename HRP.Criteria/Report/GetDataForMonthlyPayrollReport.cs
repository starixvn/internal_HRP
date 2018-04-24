namespace HRP.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    public class GetDataForMonthlyPayrollReport : CriteriaBase
    {
        public int CompanyId { get; set; }

        public DateTime Month { get; set; }

        public override string GetSettingKey() => "Report_GetDataForMonthlyPayrollReport";

        public override bool IsValid() => CompanyId > 0;
    }
}