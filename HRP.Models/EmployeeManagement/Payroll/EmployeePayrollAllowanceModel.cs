namespace HRP.Models.EmployeeManagement
{
    public class EmployeePayrollAllowanceModel : EmployeePayrollBaseBankDetails
    {
        public int EmployeeId { get; set; }

        public string CurrencyId { get; set; }

        public string SubsistenceAmount { get; set; }

        public string SubsistenceCurrencyId { get; set; }

        public string HomePassageAmount { get; set; }

        public string HomePassageCurrencyId { get; set; }

        public string HousingAmount { get; set; }

        public string HousingCurrencyId { get; set; }

        public string MobilePhoneAmount { get; set; }

        public string MobilePhoneCurrencyId { get; set; }

        public string HPIAmount { get; set; }

        public string HPICurrencyId { get; set; }

        public string DutyMealAmount { get; set; }

        public string DutyMealCurrencyId { get; set; }

        public string SecondmentAmount { get; set; }

        public string SecondmentCurrencyId { get; set; }

        public string OthersAmount { get; set; }

        public string OthersCurrencyId { get; set; }
    }
}