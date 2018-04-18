namespace HRP.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetDepartmentByUserCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override string GetSettingKey() => "Employee_GetDepartment";

        public override bool IsValid() => UserId > 0;
    }
}