namespace HRP.Criteria.JobGrade
{
    using Fanex.Data.Repository;

    public class GetAllJobGradeCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "JobGrade_GetAll";

        public override bool IsValid() => true;
    }
}