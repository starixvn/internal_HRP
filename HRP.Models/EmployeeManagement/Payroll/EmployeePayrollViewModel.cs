namespace HRP.Models.EmployeeManagement
{
    using System.Collections.Generic;
    using HRP.Models.Currency;

    public class EmployeePayrollViewModel
    {
        public EmployeePayrollBasicSalaryModel BasicSalary { get; set; }

        public IEnumerable<CurrencyCodeModel> Currencies { get; set; }

        public IEnumerable<CurrencyExchangeRateModel> CurrencyExchangeRates { get; set; }
    }
}
