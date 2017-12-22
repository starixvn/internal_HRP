namespace HRP.Models.Currency
{
    public class CurrencyExchangeRateModel
    {
        public string FromCurrency { get; set; }

        public string ToCurrency { get; set; }

        public decimal ExchangeRate { get; set; }
    }
}