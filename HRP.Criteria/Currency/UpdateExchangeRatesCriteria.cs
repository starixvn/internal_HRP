namespace HRP.Criteria
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using HRP.Models.Currency;

    public class UpdateExchangeRatesCriteria : CommandBaseCriteria
    {
        public UpdateExchangeRatesCriteria(IEnumerable<CurrencyExchangeRateModel> exchangeRates)
        {
            var xml = new XElement("Root");
            foreach (var rate in exchangeRates)
            {
                var row = new XElement("Row");
                row.SetAttributeValue(nameof(rate.FromCurrencyId), rate.FromCurrencyId);
                row.SetAttributeValue(nameof(rate.ToCurrencyId), rate.ToCurrencyId);
                row.SetAttributeValue(nameof(rate.ExchangeRate), rate.ExchangeRate);
                xml.Add(row);
            }

            ExchangeRates = xml.ToString();
        }

        public DateTime ExchangeRateMonth { get; set; }

        public string ExchangeRates { get; set; }

        public int UpdatedBy { get; set; }

        public override string GetSettingKey() => "Currency_UpdateExchangeRates";

        public override bool IsValid() => !string.IsNullOrEmpty(ExchangeRates) && UpdatedBy > 0;
    }
}
