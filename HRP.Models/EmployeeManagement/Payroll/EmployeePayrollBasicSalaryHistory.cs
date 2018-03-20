namespace HRP.Models.EmployeeManagement.Payroll
{
    using System;

    public class EmployeePayrollBasicSalaryHistory
    {
        public int HistoryId { get; set; }

        public int EmployeeId { get; set; }

        public int CurrencyId { get; set; }

        public string Salary { get; set; }

        public DateTime EffectiveDate { get; set; }

        public int ReasonId { get; set; }
    }
}