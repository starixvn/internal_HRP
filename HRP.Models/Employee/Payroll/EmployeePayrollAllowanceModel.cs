namespace HRP.Models.Employee
{
    public class EmployeePayrollAllowanceModel : EmployeePayrollBaseBankDetails
    {
        public int EmployeeId { get; set; }

        public int? CurrencyId { get; set; }

        public CurrencyAmount Subsistence { get; set; }

        public CurrencyAmount HomePassage { get; set; }

        public CurrencyAmount Housing { get; set; }

        public CurrencyAmount MobilePhone { get; set; }

        public CurrencyAmount HPI { get; set; }

        public CurrencyAmount DutyMeal { get; set; }

        public CurrencyAmount Secondment { get; set; }

        public CurrencyAmount Others { get; set; }
    }
}