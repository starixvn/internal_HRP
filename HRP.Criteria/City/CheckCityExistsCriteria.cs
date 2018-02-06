namespace HRP.Criteria.City
{
    using Fanex.Data.Repository;

    public class CheckCityExistsCriteria : CriteriaBase
    {
        public int CityId { get; set; }

        public int CountryId { get; set; }

        public string Code { get; set; }

        public override string GetSettingKey() => "City_CheckExists";

        public override bool IsValid() => CityId >= 0 && CountryId >= 0 && !string.IsNullOrWhiteSpace(Code);
    }
}