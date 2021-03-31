namespace HRP.Models.Report
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HRP.Core.Model;

    public class DirectBankingReport
    {
        public string CompanyName { get; set; }

        public DateTime Month { get; set; }

        public IEnumerable<BankIn> DirectBankingBankIns { get; set; }

        public IEnumerable<BankIn> CashBankIns { get; set; }

        public IEnumerable<ReferenceResourceModel> PaymentTransactions { get; set; }

        public bool IsExportable => (DirectBankingBankIns != null && DirectBankingBankIns.Any())
                                    || (CashBankIns != null && CashBankIns.Any());
    }
}