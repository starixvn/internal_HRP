namespace HRP.Models.EmployeeManagement
{
    using System;

    public class EmployeePayrollBasicSalaryModel
    {
        public int EmployeeId { get; set; }

        public string Salary { get; set; }

        public int WorkingDays { get; set; }

        public string EbankingCurrencyCode { get; set; }

        public string EbankingAmount { get; set; }

        public int? EbankingAccountId { get; set; }

        public string DirectBanking1CurrencyCode { get; set; }

        public string DirectBanking1Amount { get; set; }

        public int? DirectBanking1AccountId { get; set; }

        public string DirectBanking2CurrencyCode { get; set; }

        public string DirectBanking2Amount { get; set; }

        public int? DirectBanking2AccountId { get; set; }

        public string CashCurrencyCode { get; set; }

        public string CashAmount { get; set; }

        public int? CashAccountId { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public int? ReasonId { get; set; }
    }
}