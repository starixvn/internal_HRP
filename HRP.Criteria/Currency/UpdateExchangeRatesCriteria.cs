namespace HRP.Criteria
{
    using System;

    public class UpdateExchangeRatesCriteria : BaseNonQueryCommand
    {
        public DateTime ExchangeRateMonth { get; set; }

        public string ExchangeRates { get; set; }

        public int UpdatedBy { get; set; }

        public override string GetSettingKey() => "Currency_UpdateExchangeRates";

        public override bool IsValid() => !string.IsNullOrEmpty(ExchangeRates) && UpdatedBy > 0;
    }
}