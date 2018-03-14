namespace HRP.Models.PaymentProcess
{
    using System.Collections.Generic;

    public class FileResultViewModel
    {
        public int TotalRow { get; set; }

        public IEnumerable<ValidationResult> ValidationErrors { get; set; }
    }
}