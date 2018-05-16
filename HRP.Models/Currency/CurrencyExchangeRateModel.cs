namespace HRP.Models.Currency
{
    using System;

    public class CurrencyExchangeRateModel
    {
        public int FromCurrencyId { get; set; }

        public int ToCurrencyId { get; set; }

        public decimal? ExchangeRate { get; set; }

        public decimal? ExchangeRateDisplay
        {
            get
            {
                if (ExchangeRate.HasValue)
                {
                    return Math.Round(ExchangeRate.Value, 5, MidpointRounding.AwayFromZero);
                }

                return null;
            }
        }
    }
}