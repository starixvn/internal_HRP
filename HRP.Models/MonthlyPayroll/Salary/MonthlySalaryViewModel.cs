namespace HRP.Models.MonthlyPayroll
{
    using HRP.Core.Enumerations;

    public class MonthlySalaryViewModel
    {
        public CurrencyAmountModel Salary { get; set; } = new CurrencyAmountModel();
        public PaymentTransactionModel Ebanking { get; set; } = new PaymentTransactionModel();
        public PaymentTransactionModel DirectBanking1 { get; set; } = new PaymentTransactionModel();
        public PaymentTransactionModel DirectBanking2 { get; set; } = new PaymentTransactionModel();
        public PaymentTransactionModel Cash { get; set; } = new PaymentTransactionModel();
        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }
        public SecurityData AdditionAmount { get; set; } = new SecurityData();
        public SecurityData AdditionAmountDisplay { get; set; } = new SecurityData();
        public SecurityData DeductionAmount { get; set; } = new SecurityData();
        public SecurityData DeductionAmountDisplay { get; set; } = new SecurityData();
    }
}