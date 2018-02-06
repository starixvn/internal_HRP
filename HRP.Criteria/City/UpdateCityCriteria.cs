namespace HRP.Criteria.City
{
    public class UpdateCityCriteria : CommandBaseCriteria
    {
        public int CityId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int UpdatedBy { get; set; }

        public override string GetSettingKey() => "City_Update";

        public override bool IsValid() => CityId > 0 && !string.IsNullOrWhiteSpace(Code) && UpdatedBy > 0;
    }
}