namespace HRP.Models.Report
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BankInReport
    {
        public string CompanyName { get; set; }

        public DateTime ReportedMonth { get; set; }

        public IEnumerable<BankIn> EBankingBankIns { get; set; }

        public IEnumerable<BankIn> CashBankIns { get; set; }

        public IEnumerable<string> SelectedPaymentTransactions { get; set; }

        public bool IsExportable => (EBankingBankIns != null && EBankingBankIns.Any())
                                    || (CashBankIns != null && CashBankIns.Any());
    }
}