namespace HRP.Models.EmployeeManagement
{
    public class EmployeePayrollAllowanceModel
    {
        public int EmployeeId { get; set; }

        public string CurrencyCode { get; set; }

        public string DirectBanking1CurrencyCode { get; set; }

        public string DirectBanking1Amount { get; set; }

        public int? DirectBanking1AccountId { get; set; }

        public int? DirectBanking1BankTypeId { get; set; }

        public int? DirectBanking1AccountName { get; set; }

        public string DirectBanking2CurrencyCode { get; set; }

        public string DirectBanking2Amount { get; set; }

        public int? DirectBanking2AccountId { get; set; }

        public int? DirectBanking2BankTypeId { get; set; }

        public int? DirectBanking2AccountName { get; set; }

        public string CashCurrencyCode { get; set; }

        public string CashAmount { get; set; }

        public int? CashAccountId { get; set; }

        public string AutoCalculateOn { get; set; }

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
    }
}