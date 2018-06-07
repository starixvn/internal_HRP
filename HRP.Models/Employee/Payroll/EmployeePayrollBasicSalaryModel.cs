namespace HRP.Models.Employee
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class EmployeePayrollBasicSalaryModel : EmployeePayrollBaseBankDetails
    {
        public int EmployeeId { get; set; }

        public int WorkingDays { get; set; }

        public CurrencyAmount Salary { get; set; }

        public CurrencyAmount OldSalary { get; set; }

        public PaymentTransaction Ebanking { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? EffectiveDate { get; set; }

        public int? ReasonId { get; set; }

        public bool InitFirstTime { get; set; }
    }
}