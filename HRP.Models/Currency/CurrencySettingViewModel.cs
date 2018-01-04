namespace HRP.Models.Currency
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CurrencySettingViewModel
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-yyyy}")]
        public DateTime SelectedTime { get; set; }

        public IEnumerable<CurrencyExchangeRateModel> CurrencyExchangeRates { get; set; }

        public IEnumerable<CurrencyCodeModel> Currencies { get; set; }

        public DateTime MinMonth { get; set; }

        public bool CanUpdate { get; set; }
    }
}