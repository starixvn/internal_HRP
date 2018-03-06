namespace HRP.Models.PaymentProcess
{
    public class AttendanceRecord
    {
        public string EmployeeCode { get; set; }

        public int ActualWorkingDay { get; set; }

        public double LeaveTaken { get; set; }

        public double UnpaidLeave { get; set; }

        public int LateInMinute { get; set; }

        public int EarlyOutMinute { get; set; }
    }
}