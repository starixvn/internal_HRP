namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetCityByCountryIdCriteria : CriteriaBase
    {
        public int CountryId { get; set; }

        public bool? IsActive { get; set; }

        public override bool IsValid() => CountryId > 0;

        public override string GetSettingKey() => "City_GetByCountryId";
    }
}