namespace HRP.Criteria.JobGrade
{
    using Fanex.Data.Repository;

    public class GetJobGradeByIdCriteria : CriteriaBase
    {
        public int JobGradeId { get; set; }

        public override string GetSettingKey() => "JobGrade_GetById";

        public override bool IsValid() => JobGradeId > 0;
    }
}