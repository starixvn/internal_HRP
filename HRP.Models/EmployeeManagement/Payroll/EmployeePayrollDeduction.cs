namespace HRP.Models.EmployeeManagement
{
    public class EmployeePayrollDeduction
    {
        public int EmployeeId { get; set; }

        public string RoomingChargesCurrencyCode { get; set; }

        public string RoomingChargesAmount { get; set; }

        public string FitnessFirstCurrencyCode { get; set; }

        public string FitnessFirstAmount { get; set; }
    }
}