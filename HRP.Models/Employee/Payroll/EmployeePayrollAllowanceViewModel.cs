namespace HRP.Models.Employee
{
    public class EmployeePayrollAllowanceViewModel : EmployeePayrollBaseBankDetails
    {
        public int EmployeeId { get; set; }

        public int? CurrencyId { get; set; }

        public CurrencyAmountModel Subsistence { get; set; }

        public CurrencyAmountModel HomePassage { get; set; }

        public CurrencyAmountModel Housing { get; set; }

        public CurrencyAmountModel MobilePhone { get; set; }

        public CurrencyAmountModel HPI { get; set; }

        public CurrencyAmountModel DutyMeal { get; set; }

        public CurrencyAmountModel Secondment { get; set; }

        public CurrencyAmountModel Others { get; set; }

        public SecurityData DutyMealPerDay { get; set; }
    }
}