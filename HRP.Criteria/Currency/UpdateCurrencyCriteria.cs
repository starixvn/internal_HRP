namespace HRP.Criteria.Currency
{
    public class UpdateCurrencyCriteria : BaseNonQueryCommand
    {
        public int CurrencyId { get; set; }

        public string CurrencyCode { get; set; }

        public string CurrencyName { get; set; }

        public bool IsActive { get; set; }

        public int UpdatedBy { get; set; }

        public override bool IsValid() => CurrencyId > 0 && !string.IsNullOrWhiteSpace(CurrencyCode) && UpdatedBy > 0;

        public override string GetSettingKey() => "Currency_Update";
    }
}