namespace HRP.Criteria.JobCategory
{
    public class CreateJobCategoryCriteria : CommandBaseCriteria
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public override bool IsValid() => CreatedBy > 0 && !string.IsNullOrWhiteSpace(Name);

        public override string GetSettingKey() => "JobCategory_Create";
    }
}