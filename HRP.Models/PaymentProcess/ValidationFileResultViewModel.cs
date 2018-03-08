namespace HRP.Models.PaymentProcess
{
    using System.Collections.Generic;
    using HRP.Core.Model;

    public class ValidationFileResultViewModel
    {
        public int TotalRow { get; set; }

        public IEnumerable<ValidationResult> ValidationErrors { get; set; }
    }
}