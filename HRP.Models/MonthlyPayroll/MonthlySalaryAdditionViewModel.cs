namespace HRP.Models.MonthlyPayroll
{
    public class MonthlySalaryAdditionViewModel
    {
        public int PayrollId { get; set; }
        public int Type { get; set; }
        public decimal UnitAmount { get; set; }
        public decimal NoOfDays { get; set; }
        public string NetSalary { get; set; }
        public string Description { get; set; }
    }
}