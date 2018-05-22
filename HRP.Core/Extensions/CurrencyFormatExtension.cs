namespace HRP.Core.Extensions
{
    using System;
    using System.Globalization;

    public static class CurrencyFormatExtension
    {
        public static string ToStringWithCurrency(this decimal value, string currencyCode)
        {
            if (string.Compare(currencyCode, Constants.Constants.VNDCurrencyCode, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }

            return value.ToString("F2");
        }
    }
}