namespace HRP.Criteria.GradeLevel
{
    public class CreateGradeLevelCriteria : CommandBaseCriteria
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int JobCategoryId { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "GradeLevel_Create";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Name) && JobCategoryId > 0 && CreatedBy > 0;
    }
}