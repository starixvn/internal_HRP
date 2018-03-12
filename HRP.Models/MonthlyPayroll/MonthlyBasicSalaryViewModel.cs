namespace HRP.Models.MonthlyPayroll
{
    public class MonthlyBasicSalaryViewModel
    {
        public int PayrollId { get; set; }
        public string CurrencyCode { get; set; }
        public string Salary { get; set; }
        public int WorkingDays { get; set; }
        public string EBankingCurrencyCode { get; set; }
        public string EBankingAmount { get; set; }
        public string DirectBanking1CurrencyCode { get; set; }
        public string DirectBanking1Amount { get; set; }
        public string DirectBanking2CurrencyCode { get; set; }
        public string DirectBanking2Amount { get; set; }
        public string CashCurrencyCode { get; set; }
        public string CashAmount { get; set; }
        public string AutoCalculateOn { get; set; }
    }
}