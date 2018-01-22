namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetLatestExchangeRatesCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Currency_GetLatestExchangeRates";

        public override bool IsValid() => true;
    }
}
