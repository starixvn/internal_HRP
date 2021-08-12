namespace HRP.Models.PaymentProcess
{
    using System.Collections.Generic;
    using NPOI.SS.UserModel;
    using HRP.Models.Currency;

    public class AdditionDeductionTemplate
    {
        public string CompanyCode { get; set; }
        public string FileName { get; set; }
        public ISheet Sheet { get; set; }

        public IEnumerable<string> EmployeeCodes { get; set; } = new List<string>();
        public IEnumerable<Currency> Currencies { get; set; } = new List<Currency>();
        public IEnumerable<AdditionDeductionUploading> Records { get; set; } = new List<AdditionDeductionUploading>();
        public IEnumerable<ValidationResult> ValidationResults { get; set; } = new List<ValidationResult>();
    }
}