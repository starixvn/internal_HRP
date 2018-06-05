namespace HRP.Models.Employee
{
    using HRP.Core.CustomValidations;
    using HRP.Core.Enumerations;
    using HRP.Resources;

    public class EmployeePayrollBaseBankDetails
    {
        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking1Amount), nameof(DirectBanking1AccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollCurrencyRequired))]
        public int? DirectBanking1CurrencyId { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking1CurrencyId), nameof(DirectBanking1AccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollSettingAmountRequired))]
        public string DirectBanking1Amount { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking1CurrencyId), nameof(DirectBanking1Amount) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollAccountRequired))]
        public int? DirectBanking1AccountId { get; set; }

        public string DirectBanking1AccountName { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking2Amount), nameof(DirectBanking2AccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollCurrencyRequired))]
        public int? DirectBanking2CurrencyId { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking2CurrencyId), nameof(DirectBanking2AccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollSettingAmountRequired))]
        public string DirectBanking2Amount { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking2CurrencyId), nameof(DirectBanking2Amount) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollAccountRequired))]
        public int? DirectBanking2AccountId { get; set; }

        public string DirectBanking2AccountName { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(CashAmount), nameof(CashAccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollCurrencyRequired))]
        public int? CashCurrencyId { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(CashCurrencyId), nameof(CashAccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollSettingAmountRequired))]
        public string CashAmount { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(CashCurrencyId), nameof(CashAmount) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollAccountRequired))]
        public int? CashAccountId { get; set; }

        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }
    }
}