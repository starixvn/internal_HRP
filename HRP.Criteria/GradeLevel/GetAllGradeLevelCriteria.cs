namespace HRP.Criteria.GradeLevel
{
    using Fanex.Data.Repository;

    public class GetAllGradeLevelCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "GradeLevel_GetAll";
    }
}