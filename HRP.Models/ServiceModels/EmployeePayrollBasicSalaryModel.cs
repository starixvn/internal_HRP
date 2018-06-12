namespace HRP.Models.ServiceModels
{
    using System;

    public class EmployeePayrollBasicSalaryModel : EmployeePaymentTransactionBase
    {
        public int EmployeeId { get; set; }

        public string Salary { get; set; }

        public string OldSalary { get; set; }

        public int? CurrencyId { get; set; }

        public int? OldCurrencyId { get; set; }

        public int WorkingDays { get; set; }

        public int? EbankingCurrencyId { get; set; }

        public string EbankingAmount { get; set; }

        public int? EbankingAccountId { get; set; }

        public string EbankingAccountName { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public int? ReasonId { get; set; }

        public bool InitFirstTime { get; set; }
    }
}