namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetActiveCurrencyCodesCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Currency_GetActiveCurrencyCodes";

        public override bool IsValid() => true;
    }
}