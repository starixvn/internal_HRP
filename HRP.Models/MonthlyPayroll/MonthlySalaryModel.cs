namespace HRP.Models.MonthlyPayroll
{
    using HRP.Core.Enumerations;

    public class MonthlySalaryModel
    {
        public int PayrollId { get; set; }

        public int CurrencyId { get; set; }

        public string Salary { get; set; }

        public int? EBankingCurrencyId { get; set; }

        public string EBankingAmount { get; set; }

        public int? EBankingAccountId { get; set; }

        public int? CashCurrencyId { get; set; }

        public string CashAmount { get; set; }

        public int? CashAccountId { get; set; }

        public int? DirectBanking1CurrencyId { get; set; }

        public string DirectBanking1Amount { get; set; }

        public int? DirectBanking1BankTypeId { get; set; }

        public int? DirectBanking1AccountId { get; set; }

        public int? DirectBanking2CurrencyId { get; set; }

        public string DirectBanking2Amount { get; set; }

        public int? DirectBanking2BankTypeId { get; set; }

        public int? DirectBanking2AccountId { get; set; }

        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }

        public string AdditionAmount { get; set; }

        public string DeductionAmount { get; set; }

        public MonthlyPayrollStatus Status { get; set; }
    }
}