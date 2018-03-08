namespace HRP.Models.PaymentProcess
{
    public class Attendance
    {
        public string EmployeeCode { get; set; }

        public double ActualWorkingDay { get; set; }

        public double LeaveTaken { get; set; }

        public double UnpaidLeave { get; set; }

        public int LateInMinute { get; set; }

        public int EarlyOutMinute { get; set; }
    }
}