namespace HRP.Criteria.PaymentProcess
{
    using System;

    public class UploadFilesCommand : BaseNonQueryCommand
    {
        public int CompanyId { get; set; }
        public DateTime Month { get; set; }

        public string AttendanceSessionId { get; set; }
        public string AttendanceFileName { get; set; }
        public string AttendanceUploadingXMLDoc { get; set; }
        public string AttendanceResultXMLDoc { get; set; }

        public string DSTNetSalarySessionId { get; set; }
        public string DSTNetSalaryFileName { get; set; }
        public string DSTNetSalaryUploadingXMLDoc { get; set; }
        public string DSTNetSalaryResultXMLDoc { get; set; }

        public string AdditionDeductionSessionId { get; set; }
        public string AdditionDeductionFileName { get; set; }
        public string AdditionDeductionUploadingXMLDoc { get; set; }
        public string AdditionDeductionResultXMLDoc { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "PaymentProcess_UploadFiles";

        public override bool IsValid() => CompanyId > 0 && CreatedBy > 0;
    }
}