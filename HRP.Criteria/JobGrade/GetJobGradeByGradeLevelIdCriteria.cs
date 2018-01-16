namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetJobGradeByGradeLevelIdCriteria : CriteriaBase
    {
        public int GradeLevelId { get; set; }

        public bool? IsActive { get; set; }

        public override bool IsValid() => GradeLevelId > 0;

        public override string GetSettingKey() => "JobGrade_GetByGradeLevelId";
    }
}