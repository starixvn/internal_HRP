namespace HRP.Criteria.GradeLevel
{
    public class UpdateGradeLevelCriteria : BaseNonQueryCommand
    {
        public int GradeLevelId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int UpdatedBy { get; set; }

        public override string GetSettingKey() => "GradeLevel_Update";

        public override bool IsValid() => GradeLevelId > 0 && !string.IsNullOrWhiteSpace(Name) && UpdatedBy > 0;
    }
}