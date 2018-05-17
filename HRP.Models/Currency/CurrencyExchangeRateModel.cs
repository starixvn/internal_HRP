namespace HRP.Models.Currency
{
    using System;

    public class CurrencyExchangeRateModel
    {
        public int FromCurrencyId { get; set; }

        public int ToCurrencyId { get; set; }

        public decimal? ExchangeRate { get; set; }

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