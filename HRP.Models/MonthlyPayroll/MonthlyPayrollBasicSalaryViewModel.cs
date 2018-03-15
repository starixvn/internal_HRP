namespace HRP.Models.MonthlyPayroll
{
    using HRP.Core.Enumerations;

    public class MonthlyPayrollBasicSalaryViewModel
    {
        public int PayrollId { get; set; }
        public int CurrencyId { get; set; }
        public string Salary { get; set; }
        public int EbankingCurrencyId { get; set; }
        public string EbankingCurrencyCode { get; set; }
        public string EBankingAmount { get; set; }
        public int DirectBanking1CurrencyId { get; set; }
        public string DirectBanking1CurrencyCode { get; set; }
        public string DirectBanking1Amount { get; set; }
        public int DirectBanking2CurrencyId { get; set; }
        public string DirectBanking2CurrencyCode { get; set; }
        public string DirectBanking2Amount { get; set; }
        public int CashCurrencyId { get; set; }
        public string CashCurrencyCode { get; set; }
        public string CashAmount { get; set; }
        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }
    }
}