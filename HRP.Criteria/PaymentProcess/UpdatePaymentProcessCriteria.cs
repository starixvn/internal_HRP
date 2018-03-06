namespace HRP.Criteria.PaymentProcess
{
    using System;

    public class UpdatePaymentProcessCriteria : CommandBaseCriteria
    {
        public int CompanyId { get; set; }

        public DateTime Month { get; set; }

        public int CurrentStep { get; set; }

        public int UpdatedBy { get; set; }

        public override string GetSettingKey() => "PaymentProcess_Update";

        public override bool IsValid() => CompanyId > 0 && CurrentStep > 0 && UpdatedBy > 0;
    }
}