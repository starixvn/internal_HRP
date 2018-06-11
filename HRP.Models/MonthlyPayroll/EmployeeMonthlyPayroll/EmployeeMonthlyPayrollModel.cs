namespace HRP.Models.MonthlyPayroll.EmployeeMonthlyPayroll
{
    using System;
    using HRP.Core.Enumerations;

    [Serializable]
    public class EmployeeMonthlyPayrollModel
    {
        public int PayrollId { get; set; }

        public int EmployeeId { get; set; }

        public string IDNo { get; set; }

        public string Nickname { get; set; }

        public string FullName { get; set; }

        public string CostCenter { get; set; }

        public EmployeeMonthlyPayrollBasicSalaryModel BasicSalary { get; set; } = new EmployeeMonthlyPayrollBasicSalaryModel();

        public EmployeeMonthlyPayrollBasicSalaryModel Allowance { get; set; } = new EmployeeMonthlyPayrollBasicSalaryModel();

        public int WorkingDays { get; set; }

        public bool IsGenerated { get; set; }

        public bool HasSettingSalary { get; set; }

        public bool HasSettingAllowance { get; set; }

        public MonthlyPayrollStatus Status { get; set; }

        public Policy Policy { get; set; }
    }
}