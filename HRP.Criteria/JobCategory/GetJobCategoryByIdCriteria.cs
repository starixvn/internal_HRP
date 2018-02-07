namespace HRP.Criteria.JobCategory
{
    using Fanex.Data.Repository;

    public class GetJobCategoryByIdCriteria : CriteriaBase
    {
        public int JobCategoryId { get; set; }

        public override bool IsValid() => JobCategoryId > 0;

        public override string GetSettingKey() => "JobCategory_GetById";
    }
}