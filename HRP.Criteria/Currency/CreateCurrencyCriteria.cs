namespace HRP.Criteria.Currency
{
    public class CreateCurrencyCriteria : BaseNonQueryCommand
    {
        public string CurrencyCode { get; set; }

        public string CurrencyName { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public override bool IsValid() => CreatedBy > 0 && !string.IsNullOrWhiteSpace(CurrencyCode);

        public override string GetSettingKey() => "Currency_Create";
    }
}