namespace HRP.Models.PaymentProcess
{
    using HRP.Core.Extensions;

    [XMLName("Row")]
    public class DSTNetSalaryUploading
    {
        public int RowNumber { get; set; }

        public string EmployeeCode { get; set; }

        public string Currency { get; set; }

        public string NetSalaryReceived { get; set; }
    }
}