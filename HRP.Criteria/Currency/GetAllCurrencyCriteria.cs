namespace HRP.Criteria.Currency
{
    using Fanex.Data.Repository;

    public class GetAllCurrencyCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "Currency_GetAll";
    }
}