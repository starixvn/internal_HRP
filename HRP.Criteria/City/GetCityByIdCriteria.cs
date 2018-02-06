namespace HRP.Criteria.City
{
    using Fanex.Data.Repository;

    public class GetCityByIdCriteria : CriteriaBase
    {
        public int CityId { get; set; }

        public override string GetSettingKey() => "City_GetById";

        public override bool IsValid() => CityId > 0;
    }
}