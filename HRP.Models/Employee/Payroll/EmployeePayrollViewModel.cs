namespace HRP.Models.Employee.Payroll
{
    using System.Collections.Generic;
    using Currency;
    using HRP.Core.Model;

    public class EmployeePayrollViewModel
    {
        public int EmployeeId { get; set; }

        public string FullName { get; set; }

        public string Nickname { get; set; }

        public string Code { get; set; }

        public int CompanyId { get; set; }

        public EmployeePayrollBasicSalaryModel BasicSalary { get; set; } = new EmployeePayrollBasicSalaryModel();

        public EmployeePayrollAllowanceModel Allowance { get; set; } = new EmployeePayrollAllowanceModel();

        public EmployeePayrollDeductionModel Deduction { get; set; } = new EmployeePayrollDeductionModel();

        public IEnumerable<CurrencyCodeModel> Currencies { get; set; }

        public IEnumerable<CurrencyExchangeRateModel> CurrencyExchangeRates { get; set; }

        public IEnumerable<ReferenceResourceModel> ChangeReasons { get; set; }

        public IEnumerable<ReferenceResourceModel> CashAccounts { get; set; }

        public IEnumerable<ReferenceResourceModel> BankTypes { get; set; }

        public bool CanRead { get; set; }

        public bool CanUpdate { get; set; }

        public bool HaveHostPermissionOnBankAccounts { get; set; }

        public CachingFilterViewModel CachingFilter { get; set; }

        public int? VNDCurrencyId { get; set; }

        public string RijndaelIV { get; set; }

        public string RijndaelKey { get; set; }
    }
}