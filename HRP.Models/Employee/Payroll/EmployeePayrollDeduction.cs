namespace HRP.Models.Employee
{
    public class EmployeePayrollDeductionModel
    {
        public int EmployeeId { get; set; }

        public int? RoomingChargesCurrencyId { get; set; }

        public string RoomingChargesAmount { get; set; }

        public int? FitnessFirstCurrencyId { get; set; }

        public string FitnessFirstAmount { get; set; }
    }
}