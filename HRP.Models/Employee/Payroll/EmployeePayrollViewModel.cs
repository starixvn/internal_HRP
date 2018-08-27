namespace HRP.Models.Employee.Payroll
{
    using System.Collections.Generic;
    using Currency;
    using HRP.Core.Model;

    public class EmployeePayrollViewModel : RijndaelData
    {
        public int EmployeeId { get; set; }

        public string FullName { get; set; }

        public string Nickname { get; set; }

        public string Code { get; set; }

        public int CompanyId { get; set; }

        public EmployeePayrollBasicSalaryViewModel BasicSalary { get; set; } = new EmployeePayrollBasicSalaryViewModel();

        public EmployeePayrollAllowanceViewModel Allowance { get; set; } = new EmployeePayrollAllowanceViewModel();

        public EmployeePayrollDeductionViewModel Deduction { get; set; } = new EmployeePayrollDeductionViewModel();

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
    }
}