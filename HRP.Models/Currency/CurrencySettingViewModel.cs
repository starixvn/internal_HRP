namespace HRP.Models.Currency
{
    using System;
    using System.Collections.Generic;

    public class CurrencySettingViewModel
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public string SelectedMonth
        {
            get
            {
                return $"{Month}-{Year}";
            }
        }

        public IEnumerable<CurrencyExchangeRateModel> CurrencyExchangeRates { get; set; }

        public IEnumerable<CurrencyCodeModel> Currencies { get; set; }

        public IEnumerable<DateTime> Months { get; set; }
    }
}