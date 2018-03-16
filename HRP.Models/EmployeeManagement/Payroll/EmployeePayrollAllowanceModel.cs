namespace HRP.Models.EmployeeManagement
{
    public class EmployeePayrollAllowanceModel : EmployeePayrollBaseBankDetails
    {
        public int EmployeeId { get; set; }

        public int? CurrencyId { get; set; }

        public string SubsistenceAmount { get; set; }

        public int? SubsistenceCurrencyId { get; set; }

        public string HomePassageAmount { get; set; }

        public int? HomePassageCurrencyId { get; set; }

        public string HousingAmount { get; set; }

        public int? HousingCurrencyId { get; set; }

        public string MobilePhoneAmount { get; set; }

        public int? MobilePhoneCurrencyId { get; set; }

        public string HPIAmount { get; set; }

        public int? HPICurrencyId { get; set; }

        public string DutyMealAmount { get; set; }

        public int? DutyMealCurrencyId { get; set; }

        public string SecondmentAmount { get; set; }

        public int? SecondmentCurrencyId { get; set; }

        public string OthersAmount { get; set; }

        public int? OthersCurrencyId { get; set; }
    }
}