namespace HRP.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    public class BankInReportCriteria : CriteriaBase
    {
        public DateTime Month { get; set; }

        public int CompanyId { get; set; }

        public int PaymentType { get; set; }

        public string PaymentTransactions { get; set; }

        public override string GetSettingKey() => "Report_GetBankIn";

        public override bool IsValid() => CompanyId > 0;
    }
}