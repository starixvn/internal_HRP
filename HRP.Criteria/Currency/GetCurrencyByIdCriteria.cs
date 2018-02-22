namespace HRP.Criteria.Currency
{
    using Fanex.Data.Repository;

    public class GetCurrencyByIdCriteria : CriteriaBase
    {
        public int CurrencyId { get; set; }

        public override bool IsValid() => CurrencyId > 0;

        public override string GetSettingKey() => "Currency_GetById";
    }
}