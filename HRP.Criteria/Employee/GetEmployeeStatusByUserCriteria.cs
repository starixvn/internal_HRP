namespace HRP.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetEmployeeStatusByUserCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override string GetSettingKey() => "Employee_GetEmployeeStatus";

        public override bool IsValid() => UserId > 0;
    }
}