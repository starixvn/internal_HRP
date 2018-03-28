namespace HRP.Models.PaymentProcess
{
    using System;

    public class PaymentProcess
    {
        public int CompanyId { get; set; }

        public string CompanyCode { get; set; }

        public DateTime Month { get; set; }

        public string AttendanceFileName { get; set; }

        public int AttendanceTotalRow { get; set; }

        public bool IsAttendanceFileSubmited { get; set; }

        public string DSTNetSalaryFileName { get; set; }

        public int DSTNetSalaryTotalRow { get; set; }

        public bool IsDSTNetSalaryFileSubmited { get; set; }

        public bool IsPayrollGenerated { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime DueDate { get; set; }

        public bool IsHasCurrencySetting { get; set; }
    }
}