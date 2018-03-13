namespace HRP.Models.PaymentProcess
{
    using System;
    using System.Collections.Generic;
    using NPOI.SS.UserModel;

    public class AttendanceTemplate
    {
        public int CompanyId { get; set; }

        public string CompanyCode { get; set; }

        public DateTime Month { get; set; }

        public string FileName { get; set; }

        public ISheet Sheet { get; set; }

        public IEnumerable<string> EmployeeCodeList { get; set; }

        public IEnumerable<AttendanceUploading> Records { get; set; }

        public IEnumerable<ValidationResult> ValidationResults { get; set; }
    }
}