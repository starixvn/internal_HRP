namespace HRP.Models.MonthlyPayroll
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;
    using HRP.Core.CustomValidations;
    using HRP.Core.Enumerations;
    using HRP.Core.Extensions;
    using HRP.Resources;

    [XMLName("Row")]
    public class MonthlySalaryModel
    {
        public int PayrollId { get; set; }

        public int EmployeeId { get; set; }

        public int? CurrencyId { get; set; }

        public string CurrencyCode { get; set; }

        public string Salary { get; set; }

        [XmlIgnore]
        public decimal SalaryAmountInMYR { get; set; }

        [XmlIgnore]
        public decimal SalaryAmountInUSD { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(EbankingAmount), nameof(EbankingAccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollCurrencyRequired))]
        public int? EbankingCurrencyId { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(EbankingCurrencyId), nameof(EbankingAccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollSettingAmountRequired))]
        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayroll_InvalidSetting))]
        public string EbankingAmount { get; set; }

        [XmlIgnore]
        public decimal EbankingAmountInMYR { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(EbankingCurrencyId), nameof(EbankingAmount) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollAccountRequired))]
        public int? EbankingAccountId { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(CashAmount), nameof(CashAccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollCurrencyRequired))]
        public int? CashCurrencyId { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(CashCurrencyId), nameof(CashAccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollSettingAmountRequired))]
        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayroll_InvalidSetting))]
        public string CashAmount { get; set; }

        [XmlIgnore]
        public decimal CashAmountInMYR { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(CashCurrencyId), nameof(CashAmount) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollAccountRequired))]
        public int? CashAccountId { get; set; }

        [XmlIgnore]
        public string EbankingAccountName { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking1Amount), nameof(DirectBanking1AccountId) },
             ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollCurrencyRequired))]
        public int? DirectBanking1CurrencyId { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking1CurrencyId), nameof(DirectBanking1AccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollSettingAmountRequired))]
        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayroll_InvalidSetting))]
        public string DirectBanking1Amount { get; set; }

        [XmlIgnore]
        public decimal DirectBanking1AmountInMYR { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking1CurrencyId), nameof(DirectBanking1Amount) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollAccountRequired))]
        public int? DirectBanking1AccountId { get; set; }

        [XmlIgnore]
        public string DirectBanking1AccountName { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking2Amount), nameof(DirectBanking2AccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollCurrencyRequired))]
        public int? DirectBanking2CurrencyId { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking2CurrencyId), nameof(DirectBanking2AccountId) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollSettingAmountRequired))]
        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayroll_InvalidSetting))]
        public string DirectBanking2Amount { get; set; }

        [XmlIgnore]
        public decimal DirectBanking2AmountInMYR { get; set; }

        [RequiredNoneOrAll(RelatedProperties = new[] { nameof(DirectBanking2CurrencyId), nameof(DirectBanking2Amount) },
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeePayrollAccountRequired))]
        public int? DirectBanking2AccountId { get; set; }

        [XmlIgnore]
        public string DirectBanking2AccountName { get; set; }

        [XmlIgnore]
        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }

        [XMLName("AutoCalculateOn")]
        public string XmlAutoCalculateOn
        {
            get => AutoCalculateOn.ToString();
        }

        public string AdditionAmount { get; set; }

        public string AdditionAmountDisplay { get; set; }

        public string DeductionAmount { get; set; }

        public string DeductionAmountDisplay { get; set; }

        [XmlIgnore]
        public MonthlyPayrollStatus Status { get; set; }

        [XMLName("Status")]
        public int XmlStatus
        {
            get => (int)Status;
        }
    }
}