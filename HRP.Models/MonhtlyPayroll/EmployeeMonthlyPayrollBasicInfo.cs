namespace HRP.Models
{
    using System;

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
    }
}