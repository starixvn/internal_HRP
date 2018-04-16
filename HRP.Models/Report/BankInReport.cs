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

        public IEnumerable<BankIn> DirectBanking1BankIns { get; set; }

        public IEnumerable<BankIn> DirectBanking2BankIns { get; set; }

        public IEnumerable<BankIn> CashBankIns { get; set; }

        public bool IsExportable => EBankingBankIns.Any() || DirectBanking1BankIns.Any()
                                                            || DirectBanking2BankIns.Any()
                                                            || CashBankIns.Any();
    }
}