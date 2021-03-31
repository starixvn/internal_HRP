namespace HRP.Criteria.PaymentProcess
{
    using System;

    public class DeleteUploadedAttendanceCriteria : BaseNonQueryCommand
    {
        public int CompanyId { get; set; }

        public DateTime Month { get; set; }

        public override string GetSettingKey() => "PaymentProcess_DeleteUploadedAttendance";

        public override bool IsValid() => CompanyId > 0;
    }
}