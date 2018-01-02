namespace HRP.Models.Currency
{
    using System;
    using System.Collections.Generic;

    public class CurrencySettingViewModel
    {
        public DateTime SelectedTime { get; set; }

        public IEnumerable<CurrencyExchangeRateModel> CurrencyExchangeRates { get; set; }

        public IEnumerable<CurrencyCodeModel> Currencies { get; set; }

        public IEnumerable<DateTime> Months { get; set; }

        public bool CanUpdate { get; set; }
    }
}