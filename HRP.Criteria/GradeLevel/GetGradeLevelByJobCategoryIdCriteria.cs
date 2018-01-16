namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetGradeLevelByJobCategoryIdCriteria : CriteriaBase
    {
        public int JobCategoryId { get; set; }

        public bool? IsActive { get; set; }

        public override bool IsValid() => JobCategoryId > 0;

        public override string GetSettingKey() => "GradeLevel_GetByJobCategoryId";
    }
}