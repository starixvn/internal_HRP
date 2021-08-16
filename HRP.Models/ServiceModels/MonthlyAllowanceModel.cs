namespace HRP.Models.ServiceModels
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;
    using HRP.Core.CustomValidations;
    using HRP.Core.Enumerations;
    using HRP.Core.Extensions;
    using HRP.Resources;

    [XMLName("Row")]
    public class MonthlyAllowanceModel
    {
        public int PayrollId { get; set; }

        public int EmployeeId { get; set; }

        public int? CurrencyId { get; set; }

        public string CurrencyCode { get; set; }

        public string TotalAmount { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking1Amount), nameof(DirectBanking1AccountId) },
             ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollCurrencyRequired))]
        public int? DirectBanking1CurrencyId { get; set; }

        public string DirectBanking1CurrencyCode { get; set; }

        public string DirectBanking1AccountName { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking1CurrencyId), nameof(DirectBanking1Amount) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollAccountRequired))]
        public int? DirectBanking1AccountId { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking1CurrencyId), nameof(DirectBanking1AccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollSettingAmountRequired))]
        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayroll_InvalidSetting))]
        public string DirectBanking1Amount { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking2CurrencyId), nameof(DirectBanking2AccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollSettingAmountRequired))]

        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayroll_InvalidSetting))]
        public string DirectBanking2Amount { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking2Amount), nameof(DirectBanking2AccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollCurrencyRequired))]
        public int? DirectBanking2CurrencyId { get; set; }

        public string DirectBanking2CurrencyCode { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking2CurrencyId), nameof(DirectBanking2Amount) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollAccountRequired))]
        public int? DirectBanking2AccountId { get; set; }

        public string DirectBanking2AccountName { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(CashAmount), nameof(CashAccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollCurrencyRequired))]
        public int? CashCurrencyId { get; set; }

        public string CashCurrencyCode { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(CashCurrencyId), nameof(CashAmount) },
        ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollAccountRequired))]
        public int? CashAccountId { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(CashCurrencyId), nameof(CashAccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollSettingAmountRequired))]
        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayroll_InvalidSetting))]
        public string CashAmount { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DSTNetSalaryAmount) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollCurrencyRequired))]
        public int? DSTNetSalaryCurrencyId { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DSTNetSalaryCurrencyId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollSettingAmountRequired))]
        public string DSTNetSalaryAmount { get; set; }

        [XmlIgnore]
        public string DSTNetSalaryCurrencyCode { get; set; }

        public string TotalNetAmount { get; set; }

        public string AdditionAmount { get; set; }

        public string DeductionAmount { get; set; }

        [XmlIgnore]
        public string AdditionAmountDisplay { get; set; }

        [XmlIgnore]
        public string DeductionAmountDisplay { get; set; }

        [XmlIgnore]
        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }

        [XMLName("AutoCalculateOn")]
        public string XmlAutoCalculateOn => AutoCalculateOn.ToString();

        [XmlIgnore]
        public MonthlyPayrollStatus Status { get; set; }

        [XMLName("Status")]
        public int XmlStatus => (int)Status;

        public string AdditionComment { get; set; }
        public string DeductionComment { get; set; }
    }
}