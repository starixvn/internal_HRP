namespace HRP.Models.MonthlyPayroll
{
    using System;
    using System.Xml.Serialization;
    using HRP.Core.Enumerations;
    using HRP.Core.Extensions;

    [XMLName("Row")]
    public class MonthlyBasicSalaryModel
    {
        public int PayrollId { get; set; }
        public int EmployeeId { get; set; }
        public int? CurrencyId { get; set; }
        [XmlIgnore]
        public string CurrencyCode { get; set; }
        public string Salary { get; set; }
        public string OldSalary { get; set; }
        public int OldCurrencyId { get; set; }
        [XmlIgnore]
        public DateTime? EffectiveDate { get; set; }
        [XmlIgnore]
        public BasicSalaryChangeReason? SalaryChangeReason { get; set; }
        public int WorkingDays { get; set; }
        public int? EbankingCurrencyId { get; set; }
        [XmlIgnore]
        public string EbankingCurrencyCode { get; set; }
        public string EbankingAmount { get; set; }
        [XmlIgnore]
        public int? EbankingAccountId { get; set; }
        public int? DirectBanking1CurrencyId { get; set; }
        [XmlIgnore]
        public string DirectBanking1CurrencyCode { get; set; }
        public string DirectBanking1Amount { get; set; }
        [XmlIgnore]
        public int? DirectBanking1BankTypeId { get; set; }
        [XmlIgnore]
        public int? DirectBanking1AccountId { get; set; }
        public int? DirectBanking2CurrencyId { get; set; }
        [XmlIgnore]
        public string DirectBanking2CurrencyCode { get; set; }
        public string DirectBanking2Amount { get; set; }
        [XmlIgnore]
        public int? DirectBanking2BankTypeId { get; set; }
        [XmlIgnore]
        public int? DirectBanking2AccountId { get; set; }
        public int? CashCurrencyId { get; set; }
        [XmlIgnore]
        public string CashCurrencyCode { get; set; }
        public string CashAmount { get; set; }
        [XmlIgnore]
        public int? CashAccountId { get; set; }
        [XmlIgnore]
        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }
        [XMLName("AutoCalculateOn")]
        public string XmlAutoCalculateOn
        {
            get => AutoCalculateOn.ToString();
        }
        public string AfterAdjustmentAmount { get; set; }
        public string AfterAdjustmentReason { get; set; }
        [XmlIgnore]
        public decimal SalaryPerDay
        {
            get
            {
                decimal salaryPerDay = 0;
                decimal.TryParse(Salary, out decimal salary);

                if (WorkingDays > 0)
                {
                    salaryPerDay = salary / WorkingDays;
                    salaryPerDay = Math.Round(salaryPerDay, 6);
                }

                return salaryPerDay;
            }
        }
    }
}