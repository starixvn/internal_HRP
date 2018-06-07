namespace HRP.Models.Employee
{
    using HRP.Core.Enumerations;

    public class EmployeePayrollBaseBankDetails
    {
        public PaymentTransactionModel DirectBanking1 { get; set; }

        public PaymentTransactionModel DirectBanking2 { get; set; }

        public PaymentTransactionModel Cash { get; set; }

        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }
    }
}