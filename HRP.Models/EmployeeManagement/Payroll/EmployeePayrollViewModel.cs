namespace HRP.Models.EmployeeManagement.Payroll
{
    using System.Collections.Generic;
    using Currency;

    public class EmployeePayrollViewModel
    {
        public int EmployeeId { get; set; }

        public string FullName { get; set; }

        public string Nickname { get; set; }

        public string Code { get; set; }

        public int CompanyId { get; set; }

        public EmployeePayrollBasicSalaryModel BasicSalary { get; set; } = new EmployeePayrollBasicSalaryModel();

        public EmployeePayrollAllowanceModel Allowance { get; set; } = new EmployeePayrollAllowanceModel();

        public EmployeePayrollDeduction Deduction { get; set; } = new EmployeePayrollDeduction();

        public IEnumerable<CurrencyCodeModel> Currencies { get; set; }

        public IEnumerable<CurrencyExchangeRateModel> CurrencyExchangeRates { get; set; }

        public IEnumerable<ReferenceResourceModel> ChangeReasons { get; set; }

        public IEnumerable<ReferenceResourceModel> CashAccounts { get; set; }

        public IEnumerable<ReferenceResourceModel> BankTypes { get; set; }

        public bool CanRead { get; set; }

        public bool CanUpdate { get; set; }

        public bool HaveHostPermissionOnBankAccounts { get; set; }
    }
}