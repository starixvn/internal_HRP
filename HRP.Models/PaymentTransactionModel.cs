namespace HRP.Models
{
    using HRP.Core.CustomValidations;
    using HRP.Resources;

    public class PaymentTransactionModel : CurrencyAmountModel
    {
        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(PlainValue), nameof(AccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollCurrencyRequired))]
        public override int? CurrencyId { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(CurrencyId), nameof(AccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollSettingAmountRequired))]
        public override string PlainValue { get; set; }

        public override string EncryptedValue { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(PlainValue), nameof(CurrencyId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollAccountRequired))]
        public virtual int? AccountId { get; set; }

        public string AccountName { get; set; }
    }
}