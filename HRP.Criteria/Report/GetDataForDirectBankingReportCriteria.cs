namespace HRP.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    public class GetDataForDirectBankingReportCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public DateTime Month { get; set; }

        public int CompanyId { get; set; }

        public int PaymentType { get; set; }

        public string PaymentTransactions { get; set; }

        public override string GetSettingKey() => "Report_GetDataForDirectBankingReport";

        public override bool IsValid() => CompanyId > 0;
    }
}