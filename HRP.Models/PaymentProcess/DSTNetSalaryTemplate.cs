namespace HRP.Models.PaymentProcess
{
    using System;
    using System.Collections.Generic;
    using HRP.Models.Currency;
    using NPOI.SS.UserModel;

    public class DSTNetSalaryTemplate
    {
        public int CompanyId { get; set; }

        public string CompanyCode { get; set; }

        public DateTime Month { get; set; }

        public string FileName { get; set; }

        public ISheet Sheet { get; set; }

        public IEnumerable<string> EmployeeCodeList { get; set; }

        public IEnumerable<Currency> Currencies { get; set; }

        public IEnumerable<DSTNetSalaryUploading> Records { get; set; }

        public IEnumerable<ValidationResult> ValidationResults { get; set; }
    }
}