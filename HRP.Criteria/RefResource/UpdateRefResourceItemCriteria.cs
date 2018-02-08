namespace HRP.Criteria.RefResource
{
    public class UpdateRefResourceItemCriteria : CommandBaseCriteria
    {
        public string RefName { get; set; }

        public int ResourceId { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int UpdatedBy { get; set; }

        public override bool IsValid() =>
            ResourceId > 0
            && !string.IsNullOrWhiteSpace(RefName)
            && !string.IsNullOrWhiteSpace(Value)
            && UpdatedBy > 0;

        public override string GetSettingKey() => "RefResource_Update";
    }
}