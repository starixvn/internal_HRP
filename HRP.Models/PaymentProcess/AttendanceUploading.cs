namespace HRP.Models.PaymentProcess
{
    using HRP.Core.Extensions;

    [XMLName("Row")]
    public class AttendanceUploading : TemplateUploading
    {
        public string ActualWorkingDay { get; set; }
    }
}