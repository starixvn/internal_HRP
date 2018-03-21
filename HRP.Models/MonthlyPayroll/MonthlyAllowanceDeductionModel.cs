namespace HRP.Models.MonthlyPayroll
{
    using HRP.Core.CustomValidations;
    using HRP.Resources;

    public class MonthlyAllowanceDeductionModel
    {
        public int PayrollId { get; set; }

        public int Type { get; set; }

        public decimal? NoOfDays { get; set; }

        [RequiredIfRealtedFieldHasValue(RelatedName = nameof(NoOfDays), ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "UnitAmountRequired")]
        public string UnitAmount { get; set; }

        public string TotalAmount { get; set; }

        public string NetAllowance { get; set; }

        public int? CurrencyId { get; set; }
    }
}