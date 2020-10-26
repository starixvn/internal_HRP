namespace HRP.Models.MonthlyPayroll
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public class MonthlySalaryDeductionViewModel : SecurityData
    {
        private const int DescriptionLength = 200;

        public int Type { get; set; }
        public decimal? NoOfDays { get; set; }
        public SecurityData NetSalary { get; set; } = new SecurityData();
        public SecurityData NetSalaryDisplay { get; set; } = new SecurityData();

        [StringLength(DescriptionLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DescriptionMaxLength))]
        public string Description { get; set; }
    }
}