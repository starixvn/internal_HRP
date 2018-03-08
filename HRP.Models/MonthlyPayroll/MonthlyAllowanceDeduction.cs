namespace HRP.Models.MonthlyPayroll
{
    public class MonthlyAllowanceDeduction
    {
        public long PayrollId { get; set; }

        public int Type { get; set; }

        public decimal NoOfDays { get; set; }

        public string UnitAmount { get; set; }

        public string TotalAmount { get; set; }

        public string NetAllowance { get; set; }
    }
}
