namespace HRP.Models.Currency
{
    using System.Collections.Generic;

    public class CurrencySettingViewModel
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public IEnumerable<CurrencyExchangeRateModel> CurrencyExchangeRates { get; set; }

        public IEnumerable<CurrencyCodeModel> Currencies { get; set; }
    }
}