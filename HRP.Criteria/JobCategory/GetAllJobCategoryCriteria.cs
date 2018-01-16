namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetAllJobCategoryCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "JobCategory_GetAll";
    }
}