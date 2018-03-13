namespace HRP.Models.PaymentProcess
{
    using HRP.Core.Extensions;

    [XMLName("Row")]
    public class AttendanceUploading : TemplateUploading
    {
        public string ActualWorkingDay { get; set; }

        public string LeaveTaken { get; set; }

        public string UnpaidLeave { get; set; }

        public string LateInMinute { get; set; }

        public string EarlyOutMinute { get; set; }
    }
}