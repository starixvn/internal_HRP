namespace HRP.Models.Employee
{
    public class EmployeePayrollDeductionModel
    {
        public int EmployeeId { get; set; }

        public CurrencyAmountModel RoomingCharges { get; set; }

        public CurrencyAmountModel FitnessFirst { get; set; }
    }
}