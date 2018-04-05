namespace HRP.Models.Currency
{
    public class CurrencyExchangeRateModel
    {
        public int FromCurrencyId { get; set; }

        public int ToCurrencyId { get; set; }

        public decimal? ExchangeRate { get; set; }
    }
}