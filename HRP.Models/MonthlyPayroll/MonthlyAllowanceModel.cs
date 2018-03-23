namespace HRP.Models.MonthlyPayroll
{
    using System.Xml.Serialization;
    using HRP.Core.Enumerations;
    using HRP.Core.Extensions;

    [XMLName("Row")]
    public class MonthlyAllowanceModel
    {
        public int PayrollId { get; set; }

        public int EmployeeId { get; set; }

        public int? CurrencyId { get; set; }

        public string TotalAmount { get; set; }

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

        public int? CashCurrencyId { get; set; }

        public string CashAmount { get; set; }

        public int? CashAccountId { get; set; }

        public int? DSTNetSalaryCurrencyId { get; set; }

        public string DSTNetSalaryAmount { get; set; }

        public string TotalNetAmount { get; set; }

        [XmlIgnore]
        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }

        [XMLName("AutoCalculateOn")]
        public string XmlAutoCalculateOn
        {
            get => AutoCalculateOn.ToString();
        }

        [XmlIgnore]
        public MonthlyPayrollStatus Status { get; set; }

        [XMLName("Status")]
        public int XmlStatus
        {
            get => (int)Status;
        }
    }
}