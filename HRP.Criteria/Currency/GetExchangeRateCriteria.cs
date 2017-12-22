namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetExchangeRateCriteria : CriteriaBase
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public override string GetSettingKey() => "Currency_GetExchangeRates";

        public override bool IsValid() => Month > 0 && Month <= 12 && Year > 0;
    }
}