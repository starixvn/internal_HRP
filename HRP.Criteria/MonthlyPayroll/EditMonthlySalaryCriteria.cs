namespace HRP.Criteria.MonthlyPayroll
{
    using System;

    public class EditMonthlySalaryCriteria : CommandBaseCriteria
    {
        public int PayrollId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public DateTime Month { get; set; }
        public string Salary { get; set; }
        public int? EbankingCurrencyId { get; set; }
        public int? EbankingAccountId { get; set; }
        public string EbankingAmount { get; set; }
        public int? DirectBanking2CurrencyId { get; set; }
        public int? DirectBanking2AccountId { get; set; }
        public string DirectBanking2Amount { get; set; }
        public int? DirectBanking1CurrencyId { get; set; }
        public string DirectBanking1Amount { get; set; }
        public int? DirectBanking1AccountId { get; set; }
        public string CashAmount { get; set; }
        public int? CashCurrencyId { get; set; }
        public int? CashAccountId { get; set; }
        public string AdditionAmount { get; set; }
        public string DeductionAmount { get; set; }
        public string AdditionXml { get; set; }
        public string DeductionXml { get; set; }
        public string AutoCalculateOn { get; set; }

        public override string GetSettingKey() => "MonthlyPayroll_UpdateSalary";

        public override bool IsValid() => PayrollId > 0 && CompanyId > 0 && UserId > 0;
    }
}