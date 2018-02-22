namespace HRP.Models.EmployeeManagement
{
    public class EmployeePayrollDeduction
    {
        public int EmployeeId { get; set; }

        public string RoomingChargesCurrencyId { get; set; }

        public string RoomingChargesAmount { get; set; }

        public string FitnessFirstCurrencyId { get; set; }

        public string FitnessFirstAmount { get; set; }
    }
}