namespace HRP.Models.MonthlyPayroll
{
    using System;
    using HRP.Core.Enumerations;

    [Serializable]
    public class MonthlyPayrollEmployeeInfoViewModel
    {
        public int PayrollId { get; set; }

        public int EmployeeId { get; set; }

        public string IDNo { get; set; }

        public string Nickname { get; set; }

        public string FullName { get; set; }

        public string CostCenter { get; set; }

        public MonthlyPayrollBasicSalaryViewModel BasicSalary { get; set; } = new MonthlyPayrollBasicSalaryViewModel();

        public MonthlyPayrollBasicSalaryViewModel Allowance { get; set; } = new MonthlyPayrollBasicSalaryViewModel();

        public int WorkingDays { get; set; }

        public bool HasSettingSalary { get; set; }

        public bool HasSettingAllowance { get; set; }

        public MonthlyPayrollStatus Status { get; set; }

        public Policy Policy { get; set; }
    }
}