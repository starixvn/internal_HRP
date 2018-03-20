namespace HRP.Models.PaymentProcess
{
    using System.Collections.Generic;
    using HRP.Models.EmployeeManagement.Payroll;
    using HRP.Models.MonthlyPayroll;

    public class GeneratePayrollViewModel
    {
        public IEnumerable<EmployeePayrollBasicSalaryHistory> BasicSalaryHistory { get; set; }

        public IEnumerable<MonthlyPayroll> MonthlyPayroll { get; set; }
    }
}