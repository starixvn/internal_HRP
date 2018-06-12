namespace HRP.Models.MonthlyPayroll
{
    using HRP.Core.Enumerations;

    public class MonthlySalaryViewModel
    {
        public CurrencyAmountModel Salary { get; set; }
        public PaymentTransactionModel Ebanking { get; set; }
        public PaymentTransactionModel DirectBanking1 { get; set; }
        public PaymentTransactionModel DirectBanking2 { get; set; }
        public PaymentTransactionModel Cash { get; set; }
        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }
        public SecurityData AdditionAmount { get; set; }
        public SecurityData AdditionAmountDisplay { get; set; }
        public SecurityData DeductionAmount { get; set; }
        public SecurityData DeductionAmountDisplay { get; set; }
    }
}