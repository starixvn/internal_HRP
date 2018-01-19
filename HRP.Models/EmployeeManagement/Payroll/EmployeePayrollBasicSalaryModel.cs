namespace HRP.Models.EmployeeManagement
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class EmployeePayrollBasicSalaryModel
    {
        public int EmployeeId { get; set; }

        public string Salary { get; set; }

        public string CurrencyCode { get; set; }

        public int WorkingDays { get; set; }

        public string EbankingCurrencyCode { get; set; }

        public string EbankingAmount { get; set; }

        public int? EbankingAccountId { get; set; }

        public string EbankingAccountName { get; set; }

        public string DirectBanking1CurrencyCode { get; set; }

        public string DirectBanking1Amount { get; set; }

        public int? DirectBanking1AccountId { get; set; }

        public int? DirectBanking1BankTypeId { get; set; }

        public string DirectBanking1AccountName { get; set; }

        public string DirectBanking2CurrencyCode { get; set; }

        public string DirectBanking2Amount { get; set; }

        public int? DirectBanking2AccountId { get; set; }

        public int? DirectBanking2BankTypeId { get; set; }

        public string DirectBanking2AccountName { get; set; }

        public string CashCurrencyCode { get; set; }

        public string CashAmount { get; set; }

        public int? CashAccountId { get; set; }

        public string AutoCalculateOn { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? EffectiveDate { get; set; }

        public int? ReasonId { get; set; }
    }
}