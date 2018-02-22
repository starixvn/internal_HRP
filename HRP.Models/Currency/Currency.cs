namespace HRP.Models.Currency
{
    public class Currency
    {
        public int CurrencyId { get; set; }

        public string CurrencyCode { get; set; }

        public string CurrencyName { get; set; }

        public bool IsActive { get; set; }
    }
}