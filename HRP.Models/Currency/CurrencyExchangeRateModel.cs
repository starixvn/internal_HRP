namespace HRP.Models.Currency
{
    using System;
    using System.Globalization;
    using System.Xml.Serialization;
    using HRP.Core.Extensions;

    [Serializable]
    [XMLName("Row")]
    public class CurrencyExchangeRateModel
    {
        public int FromCurrencyId { get; set; }

        public int ToCurrencyId { get; set; }

        [XmlIgnore]
        public decimal? ExchangeRate { get; set; }

        [XMLName("ExchangeRate")]
        public string XmlExchangeRate
        {
            get => (ExchangeRate.HasValue) ? ExchangeRate.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
        }

        public decimal? GetExchangeRateDisplay(int? vndCurrencyId)
        {
            if (ExchangeRate.HasValue)
            {
                return Math.Round(ExchangeRate.Value, ToCurrencyId == vndCurrencyId ? 0 : 5, MidpointRounding.AwayFromZero);
            }

            return null;
        }
    }
}