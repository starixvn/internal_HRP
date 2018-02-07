namespace HRP.Criteria.JobCategory
{
    public class UpdateJobCategoryCriteria : CommandBaseCriteria
    {
        public int JobCategoryId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int UpdatedBy { get; set; }

        public override bool IsValid() => JobCategoryId > 0 && !string.IsNullOrWhiteSpace(Name) && UpdatedBy > 0;

        public override string GetSettingKey() => "JobCategory_Update";
    }
}