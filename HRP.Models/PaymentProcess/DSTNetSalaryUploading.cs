namespace HRP.Models.PaymentProcess
{
    using HRP.Core.Extensions;

    [XMLName("Row")]
    public class DSTNetSalaryUploading : TemplateUploading

    {
        public string Currency { get; set; }

        public string NetSalaryReceived { get; set; }
    }
}