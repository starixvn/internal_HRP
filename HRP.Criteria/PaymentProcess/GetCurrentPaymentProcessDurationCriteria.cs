namespace HRP.Criteria.PaymentProcess
{
    using Fanex.Data.Repository;

    public class GetCurrentPaymentProcessDurationCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "PaymentProcess_GetCurrentDuration";

        public override bool IsValid() => true;
    }
}