namespace HRP.Models.MonthlyPayroll
{
    public class MonthlyBasicSalaryViewModel
    {
        public int? CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public SecurityData Salary { get; set; }
        public int? EbankingCurrencyId { get; set; }
        public string EbankingCurrencyCode { get; set; }
        public SecurityData EbankingAmount { get; set; }
        public int? DirectBanking1CurrencyId { get; set; }
        public string DirectBanking1CurrencyCode { get; set; }
        public SecurityData DirectBanking1Amount { get; set; }
        public int? DirectBanking2CurrencyId { get; set; }
        public string DirectBanking2CurrencyCode { get; set; }
        public SecurityData DirectBanking2Amount { get; set; }
        public int? CashCurrencyId { get; set; }
        public string CashCurrencyCode { get; set; }
        public SecurityData CashAmount { get; set; }
        public SecurityData AfterAdjustmentAmount { get; set; }
        public string AfterAdjustmentReason { get; set; }
        public SecurityData SalaryPerDay { get; set; }
        public int WorkingDays { get; set; }
    }
}