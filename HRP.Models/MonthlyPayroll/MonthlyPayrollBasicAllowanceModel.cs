namespace HRP.Models.MonthlyPayroll
{
    using System.Xml.Serialization;
    using HRP.Core.Enumerations;
    using HRP.Core.Extensions;

    [XMLName("Row")]
    public class MonthlyPayrollBasicAllowanceModel
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

        public string SubsistenceAmount { get; set; }

        public int? SubsistenceCurrencyId { get; set; }

        public string HousingAmount { get; set; }

        public int? HousingCurrencyId { get; set; }

        public string HomePassageAmount { get; set; }

        public int? HomePassageCurrencyId { get; set; }

        public string MobilePhoneAmount { get; set; }

        public int? MobilePhoneCurrencyId { get; set; }

        public string DutyMealAmount { get; set; }

        public int? DutyMealCurrencyId { get; set; }

        public string HPIAmount { get; set; }

        public int? HPICurrencyId { get; set; }

        public string OthersAmount { get; set; }

        public int? OthersCurrencyId { get; set; }

        public string SecondmentAmount { get; set; }

        public int? SecondmentCurrencyId { get; set; }

        public int? FitnessFirstCurrencyId { get; set; }

        public string FitnessFirstAmount { get; set; }

        public int? RoomingChargesCurrencyId { get; set; }

        public string RoomingChargesAmount { get; set; }

        [XmlIgnore]
        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }

        [XMLName("AutoCalculateOn")]
        public string XmlAutoCalculateOn
        {
            get => AutoCalculateOn.ToString();
        }

        public string AfterAdjustmentAmount { get; set; }

        public string Reason { get; set; }
    }
}