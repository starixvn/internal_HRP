namespace HRP.Criteria.PaymentProcess
{
    using System;
    using Fanex.Data.Repository;

    public class GetUploadingFilesResultCriteria : CriteriaBase
    {
        public int CompanyId { get; set; }

        public DateTime Month { get; set; }

        public override string GetSettingKey() => "PaymentProcess_GetUploadingFilesResult";

        public override bool IsValid() => CompanyId > 0;
    }
}