namespace HRP.Models.MonthlyPayroll
{
    using HRP.Core.Enumerations;

    public class MonthlyPayrollBasicAllowanceViewModel
    {
        public int PayrollId { get; set; }

        public int EmployeeId { get; set; }

        public int? CurrencyId { get; set; }

        public string CurrencyCode { get; set; }

        public SecurityData TotalAmount { get; set; }

        public int? DirectBanking1CurrencyId { get; set; }

        public string DirectBanking1CurrencyCode { get; set; }

        public SecurityData DirectBanking1Amount { get; set; }

        public int? DirectBanking1AccountId { get; set; }

        public string DirectBanking1AccountName { get; set; }

        public int? DirectBanking2CurrencyId { get; set; }

        public string DirectBanking2CurrencyCode { get; set; }

        public SecurityData DirectBanking2Amount { get; set; }

        public int? DirectBanking2AccountId { get; set; }

        public string DirectBanking2AccountName { get; set; }

        public int? CashCurrencyId { get; set; }

        public string CashCurrencyCode { get; set; }

        public SecurityData CashAmount { get; set; }

        public int? CashAccountId { get; set; }

        public SecurityData SubsistenceAmount { get; set; }

        public int? SubsistenceCurrencyId { get; set; }

        public SecurityData HousingAmount { get; set; }

        public int? HousingCurrencyId { get; set; }

        public SecurityData HomePassageAmount { get; set; }

        public int? HomePassageCurrencyId { get; set; }

        public SecurityData MobilePhoneAmount { get; set; }

        public int? MobilePhoneCurrencyId { get; set; }

        public SecurityData DutyMealAmount { get; set; }

        public int? DutyMealCurrencyId { get; set; }

        public SecurityData HPIAmount { get; set; }

        public int? HPICurrencyId { get; set; }

        public SecurityData OthersAmount { get; set; }

        public int? OthersCurrencyId { get; set; }

        public SecurityData SecondmentAmount { get; set; }

        public int? SecondmentCurrencyId { get; set; }

        public int? FitnessFirstCurrencyId { get; set; }

        public SecurityData FitnessFirstAmount { get; set; }

        public int? RoomingChargesCurrencyId { get; set; }

        public SecurityData RoomingChargesAmount { get; set; }

        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }

        public SecurityData AfterAdjustmentAmount { get; set; }

        public SecurityData AfterAdjustmentAmountDisplay { get; set; }

        public string Reason { get; set; }
    }
}