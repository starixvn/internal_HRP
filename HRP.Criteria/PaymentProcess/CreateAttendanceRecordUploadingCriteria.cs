namespace HRP.Criteria.PaymentProcess
{
    using System;

    public class CreateAttendanceRecordUploadingCriteria : CommandBaseCriteria
    {
        public string SessionId { get; set; }

        public int CompanyId { get; set; }

        public DateTime Month { get; set; }

        public string FileName { get; set; }

        public string UploadingXMLDoc { get; set; }

        public string ResultXMLDoc { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "PaymentProcess_CreateAttendanceRecordUploading";

        public override bool IsValid() => CompanyId > 0 && CreatedBy > 0
            && !string.IsNullOrWhiteSpace(UploadingXMLDoc)
            && !string.IsNullOrWhiteSpace(ResultXMLDoc);
    }
}