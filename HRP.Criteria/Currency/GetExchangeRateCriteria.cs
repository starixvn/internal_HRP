namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetExchangeRateCriteria : CriteriaBase
    {
        private const int MaxMonth = 12;

        public int Month { get; set; }

        public int Year { get; set; }

        public override string GetSettingKey() => "Currency_GetExchangeRates";

        public override bool IsValid() => Month > 0 && Month <= MaxMonth && Year > 0;
    }
}