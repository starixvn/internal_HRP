namespace HRP.Criteria.JobGrade
{
    using Fanex.Data.Repository;

    public class CheckJobGradeExistsCriteria : CriteriaBase
    {
        public int JobGradeId { get; set; }

        public int GradeLevelId { get; set; }

        public string Name { get; set; }

        public override string GetSettingKey() => "JobGrade_CheckExists";

        public override bool IsValid() => GradeLevelId >= 0 && JobGradeId >= 0 && !string.IsNullOrWhiteSpace(Name);
    }
}