namespace HRP.Models.Report
{
    using System.Collections.Generic;
    using HRP.Models.Currency;
    using HRP.Models.Employee;
    using HRP.Models.MonthlyPayroll;

    public class MonthlyPayrollReport
    {
        public IEnumerable<EmployeeBasicInfo> Employees { get; set; }

        public IEnumerable<MonthlyBasicSalaryModel> MonthlyBasicSalary { get; set; }

        public IEnumerable<MonthlySalaryModel> MonthlySalary { get; set; }

        public IEnumerable<MonthlyPayrollBasicAllowanceModel> MonthlyBasicAllowance { get; set; }

        public IEnumerable<MonthlyAllowanceModel> MonthlyAllowance { get; set; }

        public IEnumerable<CurrencyExchangeRateModel> CurrencyExchangeRate { get; set; }
    }
}