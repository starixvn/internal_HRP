namespace HRP.Models.MonthlyPayroll
{
    using HRP.Core.Enumerations;

    public class MonthlyAllowanceModel
    {
        public int PayrollId { get; set; }

        public int EmployeeId { get; set; }

        public CurrencyAmountModel Total { get;set; }

        public PaymentTransactionModel DirectBanking1 { get; set; }
    
        public PaymentTransactionModel DirectBanking2 { get; set; }

        public PaymentTransactionModel Cash { get; set; }

        public CurrencyAmountModel DST { get; set; }

        public SecurityData TotalNet { get; set; }

        public SecurityData Addition { get; set; }

        public SecurityData Deduction { get; set; }

        public SecurityData AdditionDisplay { get; set; }

        public SecurityData DeductiontDisplay { get; set; }

        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }

        public MonthlyPayrollStatus Status { get; set; }
    }
}
