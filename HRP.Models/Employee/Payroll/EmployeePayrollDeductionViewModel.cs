namespace HRP.Models.Employee
{
    public class EmployeePayrollDeductionViewModel
    {
        public int EmployeeId { get; set; }

        public CurrencyAmountModel RoomingCharges { get; set; }

        public CurrencyAmountModel FitnessFirst { get; set; }
    }
}