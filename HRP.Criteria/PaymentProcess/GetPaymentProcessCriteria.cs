namespace HRP.Criteria.PaymentProcess
{
    using System;
    using Fanex.Data.Repository;

    public class GetPaymentProcessCriteria : CriteriaBase
    {
        public int CompanyId { get; set; }

        public DateTime Month { get; set; }

        public override string GetSettingKey() => "PaymentProcess_Get";

        public override bool IsValid() => CompanyId > 0;
    }
}