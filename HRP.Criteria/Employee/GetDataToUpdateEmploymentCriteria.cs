namespace HRP.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetDataToUpdateEmploymentCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public int EmployeeId { get; set; }

        public override bool IsValid() => UserId > 0 && EmployeeId > 0;

        public override string GetSettingKey() => "Employment_GetDataToUpdate";
    }
}