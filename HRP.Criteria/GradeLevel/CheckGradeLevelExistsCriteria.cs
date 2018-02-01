namespace HRP.Criteria.GradeLevel
{
    using Fanex.Data.Repository;

    public class CheckGradeLevelExistsCriteria : CriteriaBase
    {
        public int GradeLevelId { get; set; }

        public int JobCategoryId { get; set; }

        public string Name { get; set; }

        public override string GetSettingKey() => "GradeLevel_CheckExists";

        public override bool IsValid() => GradeLevelId >= 0 && JobCategoryId >= 0 && !string.IsNullOrWhiteSpace(Name);
    }
}