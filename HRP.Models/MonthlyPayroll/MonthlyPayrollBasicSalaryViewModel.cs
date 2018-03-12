namespace HRP.Models.MonthlyPayroll
{
    public class MonthlyPayrollBasicSalaryViewModel
    {
        public int PayrollId { get; set; }
        public string EbankingCurrencyCode { get; set; }
        public string EBankingAmount { get; set; }
        public string DirectBanking1CurrencyCode { get; set; }
        public string DirectBanking1Amount { get; set; }
        public string DirectBanking2CurrencyCode { get; set; }
        public string DirectBanking2Amount { get; set; }
        public string CashCurrencyCode { get; set; }
        public string CashAmount { get; set; }
    }
}