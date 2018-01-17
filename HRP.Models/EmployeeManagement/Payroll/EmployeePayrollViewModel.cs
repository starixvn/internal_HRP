namespace HRP.Models.EmployeeManagement
{
    using System.Collections.Generic;
    using HRP.Models.Currency;

    public class EmployeePayrollViewModel
    {
        public int EmployeeId { get; set; }

        public EmployeePayrollBasicSalaryModel BasicSalary { get; set; }

        public EmployeePayrollAllowanceModel Allowance { get; set; }

        public EmployeePayrollDeduction Deduction { get; set; }

        public IEnumerable<CurrencyCodeModel> Currencies { get; set; }

        public IEnumerable<CurrencyExchangeRateModel> CurrencyExchangeRates { get; set; }

        public IEnumerable<ReferenceResourceModel> ChangeReasons { get; set; }
    }
}