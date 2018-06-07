namespace HRP.Models.Employee
{
    using HRP.Core.Enumerations;

    public class EmployeePayrollBaseBankDetails
    {
        public PaymentTransaction DirectBanking1 { get; set; }

        public PaymentTransaction DirectBanking2 { get; set; }

        public PaymentTransaction Cash { get; set; }

        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }
    }
}