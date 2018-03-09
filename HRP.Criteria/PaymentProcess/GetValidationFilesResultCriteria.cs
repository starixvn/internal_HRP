namespace HRP.Criteria.PaymentProcess
{
    using System;
    using Fanex.Data.Repository;

    public class GetValidationFilesResultCriteria : CriteriaBase
    {
        public int CompanyId { get; set; }

        public DateTime Month { get; set; }

        public override string GetSettingKey() => "PaymentProcess_GetValidationFilesResult";

        public override bool IsValid() => CompanyId >= 0;
    }
}