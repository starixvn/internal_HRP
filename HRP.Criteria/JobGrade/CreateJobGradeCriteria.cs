namespace HRP.Criteria.JobGrade
{
    public class CreateJobGradeCriteria : BaseNonQueryCommand
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int GradeLevelId { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "JobGrade_Create";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Name) && GradeLevelId > 0 && CreatedBy > 0;
    }
}