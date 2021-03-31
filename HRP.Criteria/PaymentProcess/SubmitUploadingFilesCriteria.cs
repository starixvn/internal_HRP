namespace HRP.Criteria.PaymentProcess
{
    using System;

    public class SubmitUploadingFilesCriteria : BaseNonQueryCommand
    {
        public int CompanyId { get; set; }

        public DateTime Month { get; set; }

        public override string GetSettingKey() => "PaymentProcess_SubmitUploadingFiles";

        public override bool IsValid() => CompanyId > 0;
    }
}