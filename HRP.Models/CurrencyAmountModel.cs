namespace HRP.Models
{
    using HRP.Core.CustomValidations;
    using HRP.Resources;

    public class CurrencyAmountModel : SecurityData
    {
        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(PlainValue) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollCurrencyRequired))]
        public virtual int? CurrencyId { get; set; }

        public string CurrencyCode { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(PlainValue) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollSettingAmountRequired))]
        public override string PlainValue { get; set; }

        public override string EncryptedValue { get; set; }
    }
}