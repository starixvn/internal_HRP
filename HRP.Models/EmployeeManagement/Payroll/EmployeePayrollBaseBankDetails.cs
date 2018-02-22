namespace HRP.Models.EmployeeManagement
{
    using HRP.Core.CustomValidations;
    using HRP.Resources;

    public class EmployeePayrollBaseBankDetails
    {
        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking1Amount), nameof(DirectBanking1AccountId), nameof(DirectBanking1BankTypeId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollCurrencyRequried")]
        public string DirectBanking1CurrencyId { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking1CurrencyId), nameof(DirectBanking1AccountId), nameof(DirectBanking1BankTypeId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollSettingAmountRequried")]
        public string DirectBanking1Amount { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking1CurrencyId), nameof(DirectBanking1Amount), nameof(DirectBanking1BankTypeId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollAccountRequried")]
        public int? DirectBanking1AccountId { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking1CurrencyId), nameof(DirectBanking1Amount), nameof(DirectBanking1AccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollBankTypeRequried")]
        public int? DirectBanking1BankTypeId { get; set; }

        public string DirectBanking1AccountName { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking2Amount), nameof(DirectBanking2AccountId), nameof(DirectBanking2BankTypeId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollCurrencyRequried")]
        public string DirectBanking2CurrencyId { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking2CurrencyId), nameof(DirectBanking2AccountId), nameof(DirectBanking2BankTypeId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollSettingAmountRequried")]
        public string DirectBanking2Amount { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking2CurrencyId), nameof(DirectBanking2Amount), nameof(DirectBanking2BankTypeId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollAccountRequried")]
        public int? DirectBanking2AccountId { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking2CurrencyId), nameof(DirectBanking2Amount), nameof(DirectBanking2AccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollBankTypeRequried")]
        public int? DirectBanking2BankTypeId { get; set; }

        public string DirectBanking2AccountName { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(CashAmount), nameof(CashAccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollCurrencyRequried")]
        public string CashCurrencyId { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(CashCurrencyId), nameof(CashAccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollSettingAmountRequried")]
        public string CashAmount { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(CashCurrencyId), nameof(CashAmount) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeePayrollAccountRequried")]
        public int? CashAccountId { get; set; }

        public string AutoCalculateOn { get; set; }
    }
}