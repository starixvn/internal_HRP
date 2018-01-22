namespace HRP.Models.EmployeeManagement
{
    using HRP.Core.CustomValidations;
    using HRP.Resources;

    public class EmployeePayrollAllowanceModel
    {
        public int EmployeeId { get; set; }

        public string CurrencyCode { get; set; }

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

        public string SubsistenceCurrencyCode { get; set; }

        public string SubsistenceAmount { get; set; }

        public string HomePassageCurrencyCode { get; set; }

        public string HomePassageAmount { get; set; }

        public string HousingCurrencyCode { get; set; }

        public string HousingAmount { get; set; }

        public string MobilePhoneCurrencyCode { get; set; }

        public string MobilePhoneAmount { get; set; }

        public string HPICurrencyCode { get; set; }

        public string HPIAmount { get; set; }

        public string DutyMealCurrencyCode { get; set; }

        public string DutyMealAmount { get; set; }

        public string SecondmentCurrencyCode { get; set; }

        public string SecondmentAmount { get; set; }

        public string OthersCurrencyCode { get; set; }

        public string OthersAmount { get; set; }
    }
}