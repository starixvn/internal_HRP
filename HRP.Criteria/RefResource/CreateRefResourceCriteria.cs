namespace HRP.Criteria.RefResource
{
    public class CreateRefResourceCriteria : CommandBaseCriteria
    {
        public string RefName { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public override bool IsValid() =>
            !string.IsNullOrWhiteSpace(RefName)
            && !string.IsNullOrWhiteSpace(Value)
            && CreatedBy > 0;

        public override string GetSettingKey() => "RefResource_Create";
    }
}