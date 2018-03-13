namespace HRP.Criteria.PaymentProcess
{
    using System;

    public class CreateAttendanceUploadingCriteria : CommandBaseCriteria
    {
        public string SessionId { get; set; }

        public int CreatedBy { get; set; }

        public string FileName { get; set; }

        public DateTime Month { get; set; }

        public int CompanyId { get; set; }

        public string ResultXMLDoc { get; set; }

        public string UploadingXMLDoc { get; set; }

        public override string GetSettingKey() => "PaymentProcess_CreateAttendanceUploading";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(UploadingXMLDoc)
            && !string.IsNullOrWhiteSpace(ResultXMLDoc)
            && CompanyId > 0
            && CreatedBy > 0;
    }
}