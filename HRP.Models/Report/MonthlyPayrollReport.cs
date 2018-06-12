namespace HRP.Models.Report
{
    using System;
    using System.Collections.Generic;
    using HRP.Models.Currency;
    using HRP.Models.Employee;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.ServiceModels;

    public class MonthlyPayrollReport
    {
        public IEnumerable<EmployeeBasicInfo> Employees { get; set; }

        public IEnumerable<MonthlyBasicSalaryModel> MonthlyBasicSalary { get; set; }

        public IEnumerable<MonthlySalaryModel> MonthlySalary { get; set; }

        public IEnumerable<MonthlyPayrollBasicAllowanceModel> MonthlyBasicAllowance { get; set; }

        public IEnumerable<MonthlyAllowanceModel> MonthlyAllowance { get; set; }

        public IEnumerable<CurrencyExchangeRateModel> CurrencyExchangeRate { get; set; }

        public IEnumerable<Currency> CurrencyList { get; set; }

        public Company Company { get; set; }

        public DateTime Month { get; set; }
    }
}