namespace HRP.Criteria.Country
{
    using Fanex.Data.Repository;

    public class GetCountryByIdCriteria : CriteriaBase
    {
        public int CountryId { get; set; }

        public override bool IsValid() => CountryId > 0;

        public override string GetSettingKey() => "Country_GetById";
    }
}