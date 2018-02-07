namespace HRP.Criteria.JobCategory
{
    using Fanex.Data.Repository;

    public class CheckJobCategoryExistsCriteria : CriteriaBase
    {
        public int JobCategoryId { get; set; }

        public string Name { get; set; }

        public override bool IsValid() => JobCategoryId >= 0 && !string.IsNullOrWhiteSpace(Name);

        public override string GetSettingKey() => "JobCategory_CheckExists";
    }
}