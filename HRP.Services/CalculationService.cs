namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HRP.Core.Constants;
    using HRP.Models.Currency;

    public static class CalculationService
    {
        public static decimal RoundAmountWithCurrency(decimal amount, string currencyCode, int digits = 2, bool roundingThousand = true)
        {
            if (string.Equals(currencyCode, Constants.VNDCurrencyCode, StringComparison.OrdinalIgnoreCase))
            {
                 return roundingThousand ? Math.Round(amount / 1000, MidpointRounding.AwayFromZero) * 1000 : Math.Round(amount, 0, MidpointRounding.AwayFromZero);
            }

            return Math.Round(amount, digits, MidpointRounding.AwayFromZero);
        }

        public static decimal ConvertAmount(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            string amount,
            int? fromCurrencyId,
            int? toCurrencyId)
        {
            decimal.TryParse(amount, out decimal value);
            return value * GetExchangeRate(currencyExchangeRates, fromCurrencyId ?? 0, toCurrencyId ?? 0);
        }

        public static decimal ConvertAmount(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            decimal amount,
            int? fromCurrencyId,
            int? toCurrencyId)
        {
            return amount * GetExchangeRate(currencyExchangeRates, fromCurrencyId ?? 0, toCurrencyId ?? 0);
        }

        private static decimal GetExchangeRate(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            int fromCurrencyId,
            int toCurrencyId)
        {
            var currencyExchangeRate =
                currencyExchangeRates.FirstOrDefault(x => x.FromCurrencyId == fromCurrencyId && x.ToCurrencyId == toCurrencyId);

            return currencyExchangeRate?.ExchangeRate ?? 0;
        }
    }
}