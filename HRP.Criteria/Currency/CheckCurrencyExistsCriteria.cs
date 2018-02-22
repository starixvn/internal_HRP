namespace HRP.Criteria.Currency
{
    using Fanex.Data.Repository;

    public class CheckCurrencyExistsCriteria : CriteriaBase
    {
        public int CurrencyId { get; set; }

        public string CurrencyCode { get; set; }

        public override bool IsValid() => CurrencyId >= 0 && !string.IsNullOrWhiteSpace(CurrencyCode);

        public override string GetSettingKey() => "Currency_CheckExists";
    }
}