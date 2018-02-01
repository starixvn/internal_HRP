namespace HRP.Criteria.Section
{
    public class CreateSectionCriteria : CommandBaseCriteria
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int CostCenterId { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "Section_Create";

        public override bool IsValid() => CostCenterId > 0 && !string.IsNullOrWhiteSpace(Code) && CreatedBy > 0;
    }
}