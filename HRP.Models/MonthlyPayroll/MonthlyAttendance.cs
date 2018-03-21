namespace HRP.Models.MonthlyPayroll
{
    using System;

    public class MonthlyAttendance
    {
        public int EmployeeId { get; set; }

        public DateTime Month { get; set; }

        public double ActualWorkingDay { get; set; }
    }
}