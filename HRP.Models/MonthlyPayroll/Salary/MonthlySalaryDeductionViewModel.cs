namespace HRP.Models.MonthlyPayroll
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public class MonthlySalaryDeductionViewModel : SecurityData
    {
        public int Type { get; set; }
        public decimal? NoOfDays { get; set; }
        public SecurityData NetSalary { get; set; }
        public SecurityData NetSalaryDisplay { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DescriptionMaxLength))]
        public string Description { get; set; }
    }
}