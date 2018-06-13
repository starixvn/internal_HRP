namespace HRP.Models.MonthlyPayroll
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Core.CustomValidations;
    using HRP.Resources;

    public class MonthlySalaryAdditionViewModel : SecurityData
    {
        public int Type { get; set; }

        [RequiredIfRelatedFieldHasValue(RelatedName = nameof(NoOfDays), ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.UnitAmountRequired))]
        public override string PlainValue { get; set; }

        public decimal? NoOfDays { get; set; }
        public SecurityData NetSalary { get; set; } = new SecurityData();
        public SecurityData NetSalaryDisplay { get; set; } = new SecurityData();

        [StringLength(200, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DescriptionMaxLength))]
        public string Description { get; set; }
    }
}