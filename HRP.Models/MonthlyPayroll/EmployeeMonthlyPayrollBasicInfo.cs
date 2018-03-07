namespace HRP.Models.MonthlyPayroll
{
    using System;
    using HRP.Core.Enumerations;

    [Serializable]
    public class EmployeeMonthlyPayrollBasicInfo
    {
        public long PayrollId { get; set; }

        public int EmployeeId { get; set; }

        public string IDNo { get; set; }

        public string Nickname { get; set; }

        public string FullName { get; set; }

        public string CostCenter { get; set; }

        public int WorkingDays { get; set; }

        public Policy EditSalaryPolicy { get; set; }

        public Policy EditAllowancePolicy { get; set; }
    }
}