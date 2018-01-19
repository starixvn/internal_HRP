namespace HRP.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class UpdateEmployeePayrollCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public string Salary { get; set; }

        public string CurrencyCode { get; set; }

        public int WorkingDays { get; set; }

        public string EbankingCurrencyCode { get; set; }

        public string EbankingAmount { get; set; }

        public int? EbankingAccountId { get; set; }

        public string DirectBanking1CurrencyCode { get; set; }

        public string DirectBanking1Amount { get; set; }

        public int? DirectBanking1AccountId { get; set; }

        public int? DirectBanking1BankTypeId { get; set; }

        public string DirectBanking2CurrencyCode { get; set; }

        public string DirectBanking2Amount { get; set; }

        public int? DirectBanking2AccountId { get; set; }

        public int? DirectBanking2BankTypeId { get; set; }

        public string CashCurrencyCode { get; set; }

        public string CashAmount { get; set; }

        public int? CashAccountId { get; set; }

        public string AutoCalculateOn { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public int? ReasonId { get; set; }

        public string AllowanceCurrencyCode { get; set; }

        public string AllowanceDirectBanking1CurrencyCode { get; set; }

        public string AllowanceDirectBanking1Amount { get; set; }

        public int? AllowanceDirectBanking1AccountId { get; set; }

        public int? AllowanceDirectBanking1BankTypeId { get; set; }

        public int? AllowanceDirectBanking1AccountName { get; set; }

        public string AllowanceDirectBanking2CurrencyCode { get; set; }

        public string AllowanceDirectBanking2Amount { get; set; }

        public int? AllowanceDirectBanking2AccountId { get; set; }

        public int? AllowanceDirectBanking2BankTypeId { get; set; }

        public int? AllowanceDirectBanking2AccountName { get; set; }

        public string AllowanceCashCurrencyCode { get; set; }

        public string AllowanceCashAmount { get; set; }

        public int? AllowanceCashAccountId { get; set; }

        public string AllowanceAutoCalculateOn { get; set; }

        public string SubsistenceCurrencyCode { get; set; }

        public string SubsistenceAmount { get; set; }

        public string HomePassageCurrencyCode { get; set; }

        public string HomePassageAmount { get; set; }

        public string HousingCurrencyCode { get; set; }

        public string HousingAmount { get; set; }

        public string MobilePhoneCurrencyCode { get; set; }

        public string MobilePhoneAmount { get; set; }

        public string HPICurrencyCode { get; set; }

        public string HPIAmount { get; set; }

        public string DutyMealCurrencyCode { get; set; }

        public string DutyMealAmount { get; set; }

        public string SecondmentCurrencyCode { get; set; }

        public string SecondmentAmount { get; set; }

        public string OthersCurrencyCode { get; set; }

        public string OthersAmount { get; set; }

        public string RoomingChargesCurrencyCode { get; set; }

        public string RoomingChargesAmount { get; set; }

        public string FitnessFirstCurrencyCode { get; set; }

        public string FitnessFirstAmount { get; set; }

        public int UpdatedBy { get; set; }

        public override string GetSettingKey() => "Employee_UpdatePayroll";

        public override bool IsValid() => EmployeeId > 0 && UpdatedBy > 0;
    }
}