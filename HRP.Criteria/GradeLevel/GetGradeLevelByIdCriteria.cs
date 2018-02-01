namespace HRP.Criteria.GradeLevel
{
    using Fanex.Data.Repository;

    public class GetGradeLevelByIdCriteria : CriteriaBase
    {
        public int GradeLevelId { get; set; }

        public override string GetSettingKey() => "GradeLevel_GetById";

        public override bool IsValid() => GradeLevelId > 0;
    }
}