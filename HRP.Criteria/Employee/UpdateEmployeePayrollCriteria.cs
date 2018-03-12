namespace HRP.Criteria
{
    using System;
    using HRP.Core.Enumerations;

    public class UpdateEmployeePayrollCriteria : CommandBaseCriteria
    {
        public int EmployeeId { get; set; }

        public string Salary { get; set; }

        public bool IsSalaryChanged { get; set; }

        public string CurrencyId { get; set; }

        public int WorkingDays { get; set; }

        public string EbankingCurrencyId { get; set; }

        public string EbankingAmount { get; set; }

        public int? EbankingAccountId { get; set; }

        public string DirectBanking1CurrencyId { get; set; }

        public string DirectBanking1Amount { get; set; }

        public int? DirectBanking1AccountId { get; set; }

        public int? DirectBanking1BankTypeId { get; set; }

        public string DirectBanking2CurrencyId { get; set; }

        public string DirectBanking2Amount { get; set; }

        public int? DirectBanking2AccountId { get; set; }

        public int? DirectBanking2BankTypeId { get; set; }

        public string CashCurrencyId { get; set; }

        public string CashAmount { get; set; }

        public int? CashAccountId { get; set; }

        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public int? ReasonId { get; set; }

        public string AllowanceCurrencyId { get; set; }

        public string AllowanceDirectBanking1CurrencyId { get; set; }

        public string AllowanceDirectBanking1Amount { get; set; }

        public int? AllowanceDirectBanking1AccountId { get; set; }

        public int? AllowanceDirectBanking1BankTypeId { get; set; }

        public string AllowanceDirectBanking2CurrencyId { get; set; }

        public string AllowanceDirectBanking2Amount { get; set; }

        public int? AllowanceDirectBanking2AccountId { get; set; }

        public int? AllowanceDirectBanking2BankTypeId { get; set; }

        public string AllowanceCashCurrencyId { get; set; }

        public string AllowanceCashAmount { get; set; }

        public int? AllowanceCashAccountId { get; set; }

        public PayrollAutoCalculateOn AllowanceAutoCalculateOn { get; set; }

        public string SubsistenceCurrencyId { get; set; }

        public string SubsistenceAmount { get; set; }

        public string HomePassageCurrencyId { get; set; }

        public string HomePassageAmount { get; set; }

        public string HousingCurrencyId { get; set; }

        public string HousingAmount { get; set; }

        public string MobilePhoneCurrencyId { get; set; }

        public string MobilePhoneAmount { get; set; }

        public string HPICurrencyId { get; set; }

        public string HPIAmount { get; set; }

        public string DutyMealCurrencyId { get; set; }

        public string DutyMealAmount { get; set; }

        public string SecondmentCurrencyId { get; set; }

        public string SecondmentAmount { get; set; }

        public string OthersCurrencyId { get; set; }

        public string OthersAmount { get; set; }

        public string RoomingChargesCurrencyId { get; set; }

        public string RoomingChargesAmount { get; set; }

        public string FitnessFirstCurrencyId { get; set; }

        public string FitnessFirstAmount { get; set; }

        public int UpdatedBy { get; set; }

        public override string GetSettingKey() => "Employee_UpdatePayroll";

        public override bool IsValid() => EmployeeId > 0 && UpdatedBy > 0;
    }
}