namespace HRP.Criteria.Office
{
    public class CreateOfficeCriteria : CommandBaseCriteria
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int CityId { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "Office_Create";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Code) && CityId > 0 && CreatedBy > 0;
    }
}