namespace HRP.Models.MonthlyPayroll
{
    using HRP.Core.CustomValidations;
    using HRP.Resources;

    public class MonthlyAllowanceDetailAdjustmentViewModel : SecurityData
    {
        public int PayrollId { get; set; }

        public int Type { get; set; }

        public int EmployeeId { get; set; }

        public decimal? NoOfDays { get; set; }

        public int? CurrencyId { get; set; }

        public string CurrencyCode { get; set; }

        [RequiredIfRelatedFieldHasValue(RelatedName = nameof(NoOfDays), ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "UnitAmountRequired")]
        public override string PlainValue { get; set; }

        public SecurityData NetAllowance { get; set; }

        public SecurityData NetAllowanceToDisplay { get; set; }

        public SecurityData TotalAmount { get; set; }

        public SecurityData TotalAmountToDisplay { get; set; }

    }
}
