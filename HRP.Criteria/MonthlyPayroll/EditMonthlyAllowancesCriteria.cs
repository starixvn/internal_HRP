namespace HRP.Criteria.MonthlyPayroll
{
    using HRP.Core.Enumerations;

    public class EditMonthlyAllowancesCriteria : CommandBaseCriteria
    {
        public int PayrollId { get; set; }

        public int? CurrencyId { get; set; }

        public string TotalAmount { get; set; }

        public int? CashCurrencyId { get; set; }

        public string CashAmount { get; set; }

        public int? CashAccountId { get; set; }

        public int? DirectBanking1CurrencyId { get; set; }

        public string DirectBanking1Amount { get; set; }

        public int? DirectBanking1AccountId { get; set; }

        public int? DirectBanking1BankTypeId { get; set; }

        public string DirectBanking1AccountName { get; set; }

        public int? DirectBanking2CurrencyId { get; set; }

        public string DirectBanking2Amount { get; set; }

        public int? DirectBanking2AccountId { get; set; }

        public int? DirectBanking2BankTypeId { get; set; }

        public string DirectBanking2AccountName { get; set; }

        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }

        public string AllowanceAddition { get; set; }

        public string AllowanceDeduction { get; set; }

        public int UserId { get; set; }

        public override string GetSettingKey() => "MonthlyPayroll_UpdateAllowances";

        public override bool IsValid() => PayrollId > 0 && UserId > 0;
    }
}