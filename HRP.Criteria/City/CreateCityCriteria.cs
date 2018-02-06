namespace HRP.Criteria.City
{
    public class CreateCityCriteria : CommandBaseCriteria
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int CountryId { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "City_Create";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Code) && CountryId > 0 && CreatedBy > 0;
    }
}