namespace HRP.Criteria
{
    using System;
    using HRP.Core.Enumerations;

    public class UpdateEmployeePayrollCriteria : CommandBaseCriteria
    {
        public int EmployeeId { get; set; }

        public string Salary { get; set; }

        public bool IsSalaryChanged { get; set; }

        public int? CurrencyId { get; set; }

        public int WorkingDays { get; set; }

        public int? EbankingCurrencyId { get; set; }

        public string EbankingAmount { get; set; }

        public int? EbankingAccountId { get; set; }

        public int? DirectBanking1CurrencyId { get; set; }

        public string DirectBanking1Amount { get; set; }

        public int? DirectBanking1AccountId { get; set; }

        public int? DirectBanking1BankTypeId { get; set; }

        public int? DirectBanking2CurrencyId { get; set; }

        public string DirectBanking2Amount { get; set; }

        public int? DirectBanking2AccountId { get; set; }

        public int? DirectBanking2BankTypeId { get; set; }

        public int? CashCurrencyId { get; set; }

        public string CashAmount { get; set; }

        public int? CashAccountId { get; set; }

        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public int? ReasonId { get; set; }

        public int? AllowanceCurrencyId { get; set; }

        public int? AllowanceDirectBanking1CurrencyId { get; set; }

        public string AllowanceDirectBanking1Amount { get; set; }

        public int? AllowanceDirectBanking1AccountId { get; set; }

        public int? AllowanceDirectBanking1BankTypeId { get; set; }

        public int? AllowanceDirectBanking2CurrencyId { get; set; }

        public string AllowanceDirectBanking2Amount { get; set; }

        public int? AllowanceDirectBanking2AccountId { get; set; }

        public int? AllowanceDirectBanking2BankTypeId { get; set; }

        public int? AllowanceCashCurrencyId { get; set; }

        public string AllowanceCashAmount { get; set; }

        public int? AllowanceCashAccountId { get; set; }

        public PayrollAutoCalculateOn AllowanceAutoCalculateOn { get; set; }

        public int? SubsistenceCurrencyId { get; set; }

        public string SubsistenceAmount { get; set; }

        public int? HomePassageCurrencyId { get; set; }

        public string HomePassageAmount { get; set; }

        public int? HousingCurrencyId { get; set; }

        public string HousingAmount { get; set; }

        public int? MobilePhoneCurrencyId { get; set; }

        public string MobilePhoneAmount { get; set; }

        public int? HPICurrencyId { get; set; }

        public string HPIAmount { get; set; }

        public int? DutyMealCurrencyId { get; set; }

        public string DutyMealAmount { get; set; }

        public int? SecondmentCurrencyId { get; set; }

        public string SecondmentAmount { get; set; }

        public int? OthersCurrencyId { get; set; }

        public string OthersAmount { get; set; }

        public int? RoomingChargesCurrencyId { get; set; }

        public string RoomingChargesAmount { get; set; }

        public int? FitnessFirstCurrencyId { get; set; }

        public string FitnessFirstAmount { get; set; }

        public int UpdatedBy { get; set; }

        public override string GetSettingKey() => "Employee_UpdatePayroll";

        public override bool IsValid() => EmployeeId > 0 && UpdatedBy > 0;
    }
}