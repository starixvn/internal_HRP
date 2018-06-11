namespace HRP.Models.ServiceModels
{
    using System;
    using System.Collections.Generic;

    public class MonthlyAllowances
    {
        public int EmployeeId { get; set; }

        public int CompanyId { get; set; }

        public DateTime Month { get; set; }

        public MonthlyAllowance Allowance { get; set; } = new MonthlyAllowance();

        public MonthlyPayrollBasicAllowance BasicAllowance { get; set; } = new MonthlyPayrollBasicAllowance();

        public IList<MonthlyAllowanceAddition> Additions { get; set; } = new List<MonthlyAllowanceAddition>();

        public IList<MonthlyAllowanceDeduction> Deductions { get; set; } = new List<MonthlyAllowanceDeduction>();
    }
}