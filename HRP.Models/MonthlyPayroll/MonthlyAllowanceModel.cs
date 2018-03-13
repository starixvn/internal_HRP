namespace HRP.Models.MonthlyPayroll
{
    using HRP.Core.Enumerations;

    public class MonthlyAllowanceModel
    {
        public int PayrollId { get; set; }

        public string CurrencyId { get; set; }

        public string DirectBanking1CurrencyId { get; set; }

        public string DirectBanking1Amount { get; set; }

        public int? DirectBanking1AccountId { get; set; }

        public int? DirectBanking1BankTypeId { get; set; }

        public string DirectBanking1AccountName { get; set; }

        public string DirectBanking2CurrencyId { get; set; }

        public string DirectBanking2Amount { get; set; }

        public int? DirectBanking2AccountId { get; set; }

        public int? DirectBanking2BankTypeId { get; set; }

        public string DirectBanking2AccountName { get; set; }

        public string CashCurrencyId { get; set; }

        public string CashAmount { get; set; }

        public int? CashAccountId { get; set; }

        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }
    }
}