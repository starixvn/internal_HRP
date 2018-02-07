namespace HRP.Criteria.Country
{
    using Fanex.Data.Repository;

    public class CheckCountryExistsCriteria : CriteriaBase
    {
        public int CountryId { get; set; }

        public string Code { get; set; }

        public override bool IsValid() => CountryId >= 0 && !string.IsNullOrWhiteSpace(Code);

        public override string GetSettingKey() => "Country_CheckExists";
    }
}