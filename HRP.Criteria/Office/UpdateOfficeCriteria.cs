namespace HRP.Criteria.Office
{
    public class UpdateOfficeCriteria : CommandBaseCriteria
    {
        public int OfficeId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int UpdatedBy { get; set; }

        public override string GetSettingKey() => "Office_Update";

        public override bool IsValid() => OfficeId > 0 && !string.IsNullOrWhiteSpace(Code) && UpdatedBy > 0;
    }
}