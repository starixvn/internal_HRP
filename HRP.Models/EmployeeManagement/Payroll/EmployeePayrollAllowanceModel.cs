namespace HRP.Models.EmployeeManagement
{
    public class EmployeePayrollAllowanceModel : EmployeePayrollBaseBankDetails
    {
        public int EmployeeId { get; set; }

        public string CurrencyCode { get; set; }     

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