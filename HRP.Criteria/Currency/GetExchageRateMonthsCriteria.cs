namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetExchageRateMonthsCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Currency_GetExchageRateMonths";

        public override bool IsValid() => true;
    }
}