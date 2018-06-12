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
        public int? CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string Salary { get; set; }

        [XmlIgnore]
        public decimal SalaryAmountInMYR { get; set; }

        [XmlIgnore]
        public decimal SalaryAmountInUSD { get; set; }

        public int? EbankingCurrencyId { get; set; }
        public string EbankingAmount { get; set; }

        [XmlIgnore]
        public decimal EbankingAmountInMYR { get; set; }

        public int? EbankingAccountId { get; set; }
        public int? CashCurrencyId { get; set; }
        public string CashAmount { get; set; }

        [XmlIgnore]
        public decimal CashAmountInMYR { get; set; }

        public int? CashAccountId { get; set; }

        [XmlIgnore]
        public string EbankingAccountName { get; set; }

        public int? DirectBanking1CurrencyId { get; set; }
        public string DirectBanking1Amount { get; set; }

        [XmlIgnore]
        public decimal DirectBanking1AmountInMYR { get; set; }

        public int? DirectBanking1AccountId { get; set; }

        [XmlIgnore]
        public string DirectBanking1AccountName { get; set; }

        public int? DirectBanking2CurrencyId { get; set; }
        public string DirectBanking2Amount { get; set; }

        [XmlIgnore]
        public decimal DirectBanking2AmountInMYR { get; set; }

        public int? DirectBanking2AccountId { get; set; }

        [XmlIgnore]
        public string DirectBanking2AccountName { get; set; }

        [XmlIgnore]
        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }

        [XMLName("AutoCalculateOn")]
        public string XmlAutoCalculateOn => AutoCalculateOn.ToString();

        public string AdditionAmount { get; set; }
        public string AdditionAmountDisplay { get; set; }
        public string DeductionAmount { get; set; }
        public string DeductionAmountDisplay { get; set; }

        [XmlIgnore]
        public MonthlyPayrollStatus Status { get; set; }

        [XMLName("Status")]
        public int XmlStatus => (int)Status;
    }
}