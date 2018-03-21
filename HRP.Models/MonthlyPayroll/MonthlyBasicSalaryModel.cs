namespace HRP.Models.MonthlyPayroll
{
    using System;
    using System.Globalization;
    using HRP.Core.Enumerations;

    public class MonthlyBasicSalaryModel
    {
        public int PayrollId { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string Salary { get; set; }
        public string OldSalary { get; set; }
        public int OldCurrencyId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public BasicSalaryChangeReason? SalaryChangeReason { get; set; }
        public int WorkingDays { get; set; }
        public int? EBankingCurrencyId { get; set; }
        public string EBankingCurrencyCode { get; set; }
        public string EBankingAmount { get; set; }
        public int? DirectBanking1CurrencyId { get; set; }
        public string DirectBanking1CurrencyCode { get; set; }
        public string DirectBanking1Amount { get; set; }
        public int? DirectBanking2CurrencyId { get; set; }
        public string DirectBanking2CurrencyCode { get; set; }
        public string DirectBanking2Amount { get; set; }
        public int? CashCurrencyId { get; set; }
        public string CashCurrencyCode { get; set; }
        public string CashAmount { get; set; }
        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }
        public string AfterAdjustmentAmount { get; set; }
        public string AfterAdjustmentReason { get; set; }

        public string SalaryPerDay
        {
            get
            {
                decimal.TryParse(Salary, out decimal salary);
                return (salary / WorkingDays).ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}