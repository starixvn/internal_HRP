namespace HRP.Models.MonthlyPayroll
{
    using System.Xml.Serialization;
    using HRP.Core.Enumerations;
    using HRP.Core.Extensions;

    [XMLName("Row")]
    public class MonthlySalaryModel
    {
        public int PayrollId { get; set; }

        public int EmployeeId { get; set; }

        public int CurrencyId { get; set; }

        public string Salary { get; set; }

        public int? EbankingCurrencyId { get; set; }

        public string EbankingAmount { get; set; }

        public int? EbankingAccountId { get; set; }

        public int? CashCurrencyId { get; set; }

        public string CashAmount { get; set; }

        public int? CashAccountId { get; set; }

        [XmlIgnore]
        public string EbankingAccountName { get; set; }

        public int? DirectBanking1CurrencyId { get; set; }

        public string DirectBanking1Amount { get; set; }

        public int? DirectBanking1BankTypeId { get; set; }

        public int? DirectBanking1AccountId { get; set; }

        [XmlIgnore]
        public string DirectBanking1AccountName { get; set; }

        public int? DirectBanking2CurrencyId { get; set; }

        public string DirectBanking2Amount { get; set; }

        public int? DirectBanking2BankTypeId { get; set; }

        public int? DirectBanking2AccountId { get; set; }

        [XmlIgnore]
        public string DirectBanking2AccountName { get; set; }

        [XmlIgnore]
        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }

        [XMLName("AutoCalculateOn")]
        public string XmlAutoCalculateOn
        {
            get => AutoCalculateOn.ToString();
        }

        public string AdditionAmount { get; set; }

        public string AdditionAmountDisplay { get; set; }

        public string DeductionAmount { get; set; }

        public string DeductionAmountDisplay { get; set; }

        [XmlIgnore]
        public MonthlyPayrollStatus Status { get; set; }

        [XMLName("Status")]
        public int XmlStatus
        {
            get => (int)Status;
        }
    }
}