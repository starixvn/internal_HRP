namespace HRP.Criteria.Section
{
    public class UpdateSectionCriteria : BaseNonQueryCommand
    {
        public int SectionId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int UpdatedBy { get; set; }

        public override string GetSettingKey() => "Section_Update";

        public override bool IsValid() => SectionId > 0 && !string.IsNullOrWhiteSpace(Code) && UpdatedBy > 0;
    }
}