namespace HRP.Models.EmployeeManagement
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Core.CustomValidations;
    using HRP.Resources;

    public class EmployeePayrollBasicSalaryModel
    {
        public int EmployeeId { get; set; }

        public string Salary { get; set; }

        public string OldSalary { get; set; }

        public string CurrencyCode { get; set; }

        public int WorkingDays { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(EbankingAmount), nameof(EbankingAccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollCurrencyRequried")]
        public string EbankingCurrencyCode { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(EbankingCurrencyCode), nameof(EbankingAccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollSettingAmountRequried")]
        public string EbankingAmount { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(EbankingCurrencyCode), nameof(EbankingAmount) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollAccountRequried")]
        public int? EbankingAccountId { get; set; }

        public string EbankingAccountName { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking1Amount), nameof(DirectBanking1AccountId), nameof(DirectBanking1BankTypeId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollCurrencyRequried")]
        public string DirectBanking1CurrencyCode { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking1CurrencyCode), nameof(DirectBanking1AccountId), nameof(DirectBanking1BankTypeId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollSettingAmountRequried")]
        public string DirectBanking1Amount { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking1CurrencyCode), nameof(DirectBanking1Amount), nameof(DirectBanking1BankTypeId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollAccountRequried")]
        public int? DirectBanking1AccountId { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking1CurrencyCode), nameof(DirectBanking1Amount), nameof(DirectBanking1AccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollBankTypeRequried")]
        public int? DirectBanking1BankTypeId { get; set; }

        public string DirectBanking1AccountName { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking2Amount), nameof(DirectBanking2AccountId), nameof(DirectBanking2BankTypeId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollCurrencyRequried")]
        public string DirectBanking2CurrencyCode { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking2CurrencyCode), nameof(DirectBanking2AccountId), nameof(DirectBanking2BankTypeId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollSettingAmountRequried")]
        public string DirectBanking2Amount { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking2CurrencyCode), nameof(DirectBanking2Amount), nameof(DirectBanking2BankTypeId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollAccountRequried")]
        public int? DirectBanking2AccountId { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking2CurrencyCode), nameof(DirectBanking2Amount), nameof(DirectBanking2AccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollBankTypeRequried")]
        public int? DirectBanking2BankTypeId { get; set; }

        public string DirectBanking2AccountName { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(CashAmount), nameof(CashAccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollCurrencyRequried")]
        public string CashCurrencyCode { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(CashCurrencyCode), nameof(CashAccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollSettingAmountRequried")]
        public string CashAmount { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(CashCurrencyCode), nameof(CashAmount) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollAccountRequried")]
        public int? CashAccountId { get; set; }

        public string AutoCalculateOn { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? EffectiveDate { get; set; }

        public int? ReasonId { get; set; }

        public bool InitFirstTime { get; set; }
    }
}