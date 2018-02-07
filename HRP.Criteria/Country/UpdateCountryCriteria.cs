namespace HRP.Criteria.Country
{
    public class UpdateCountryCriteria : CommandBaseCriteria
    {
        public int CountryId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int UpdatedBy { get; set; }

        public override bool IsValid() => CountryId > 0 && !string.IsNullOrWhiteSpace(Code) && UpdatedBy > 0;

        public override string GetSettingKey() => "Country_Update";
    }
}