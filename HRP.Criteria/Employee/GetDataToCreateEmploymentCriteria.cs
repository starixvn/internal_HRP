namespace HRP.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetDataToCreateEmploymentCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override string GetSettingKey() => "Employment_GetDataToCreate";

        public override bool IsValid() => UserId > 0;
    }
}