namespace HRP.Criteria.JobGrade
{
    public class UpdateJobGradeCriteria : CommandBaseCriteria
    {
        public int JobGradeId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int UpdatedBy { get; set; }

        public override string GetSettingKey() => "JobGrade_Update";

        public override bool IsValid() => JobGradeId > 0 && !string.IsNullOrWhiteSpace(Name) && UpdatedBy > 0;
    }
}