namespace HRP.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    public class GetDataForEbankingReportCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public DateTime Month { get; set; }

        public int CompanyId { get; set; }

        public string PaymentTransactions { get; set; }

        public override string GetSettingKey() => "Report_GetDataForEbankingReport";

        public override bool IsValid() => CompanyId > 0;
    }
}