namespace HRP.Models.MonthlyPayroll
{
    using System;

    public class MonthlyPayroll
    {
        public int EmployeeId { get; set; }

        public DateTime Month { get; set; }

        public int Status { get; set; }

        public DateTime SalaryEffectiveDate { get; set; }

        public int SalaryChangeReasonId { get; set; }

        public DateTime GroupStartDate { get; set; }

        public DateTime GroupEndDate { get; set; }
    }
}