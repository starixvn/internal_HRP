namespace HRP.Criteria.MonthlyPayroll
{
    using HRP.Core.Enumerations;

    public class EditMonthlyAllowancesCommand : BaseNonQueryCommand
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

        public int? DirectBanking2CurrencyId { get; set; }
        public string DirectBanking2Amount { get; set; }
        public int? DirectBanking2AccountId { get; set; }

        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }

        public string TotalNetAmount { get; set; }

        public int? DSTNetSalaryCurrencyId { get; set; }
        public string DSTNetSalaryAmount { get; set; }

        public string AdditionXml { get; set; }
        public string DeductionXml { get; set; }

        public string AdditionAmount { get; set; }
        public string AdditionComment { get; set; }

        public string DeductionAmount { get; set; }
        public string DeductionComment { get; set; }

        public MonthlyPayrollStatus Status { get; set; }

        public int UserId { get; set; }

        public override string GetSettingKey() => "MonthlyPayroll_UpdateAllowances";

        public override bool IsValid() => PayrollId > 0 && UserId > 0;
    }
}