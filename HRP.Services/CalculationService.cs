namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using HRP.Core.Constants;
    using HRP.Models.Currency;

    public static class CalculationService
    {
        /// <summary>
        /// Round amount based on currency
        /// </summary>
        /// <param name="amount">amount</param>
        /// <param name="currencyCode">currency code</param>
        /// <param name="digits">number of decimal place</param>
        /// <param name="roundingThousand">In case currency is VND, round the amount to thousand or zero</param>
        /// <returns></returns>
        private const int Thousand = 1000;

        public static decimal RoundAmountWithCurrency(decimal amount, string currencyCode, int digits = 2, bool roundingThousand = true)
        {
            if (string.Equals(currencyCode, Constants.VNDCurrencyCode, StringComparison.OrdinalIgnoreCase))
            {
                return roundingThousand ? Math.Round(amount / Thousand, MidpointRounding.AwayFromZero) * Thousand : Math.Round(amount, 0, MidpointRounding.AwayFromZero);
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

        public static string FormatAmount(decimal amount, string currencyCode)
        {
            var result = amount.ToString("#,##0.00");

            if (string.Equals(currencyCode, Constants.VNDCurrencyCode, StringComparison.OrdinalIgnoreCase))
            {
                result = amount.ToString("#,##0");
            }

            return result;
        }

        public static string FormatAmount(string amount, string currencyCode)
        {
            var result = string.Empty;

            if (!string.IsNullOrWhiteSpace(amount))
            {
                decimal.TryParse(amount, out decimal number);
                result = number.ToString("#,##0.00");

                if (string.Equals(currencyCode, Constants.VNDCurrencyCode, StringComparison.OrdinalIgnoreCase))
                {
                    result = number.ToString("#,##0");
                }
            }

            return result;
        }

        public static string GetPartOfAfterAdjustmentReason(
            DateTime workingMonth,
            DateTime groupStartDate,
            DateTime? groupEndDate)
        {
            int differenceEndDate = groupEndDate.HasValue ? GetDifferenceMonth(groupEndDate.Value, workingMonth) : -1;
            var reason = new StringBuilder();
            if (GetDifferenceMonth(groupStartDate, workingMonth) == 0)
            {
                if (differenceEndDate == 0)
                {
                    reason.AppendFormat(
                        " Employee worked from {0} to {1}.",
                        groupStartDate.ToString(Constants.DateFormat),
                        groupEndDate.Value.ToString(Constants.DateFormat));
                }
                else
                {
                    reason.AppendFormat(" Employee started working from {0}.", groupStartDate.ToString(Constants.DateFormat));
                }
            }
            else
            {
                if (differenceEndDate == 0)
                {
                    reason.AppendFormat(" Employee official leaves from {0}.", groupEndDate.Value.ToString(Constants.DateFormat));
                }
            }

            return reason.ToString();
        }

        public static int GetDifferenceMonth(DateTime past, DateTime now)
        {
            int monthsApart = 12 * (past.Year - now.Year) + past.Month - now.Month;
            return Math.Abs(monthsApart);
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