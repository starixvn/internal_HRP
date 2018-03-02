namespace HRP.Models.PaymentProcess
{
    using System;

    public class PaymentProcess
    {
        public int CompanyId { get; set; }

        public DateTime PaymentMonth { get; set; }

        public int CurrentStep { get; set; }

        public string AttendanceFileName { get; set; }

        public string DSTNetSalaryFileName { get; set; }
    }
}