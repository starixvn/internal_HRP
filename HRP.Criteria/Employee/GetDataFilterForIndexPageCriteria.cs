namespace HRP.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetDataFilterForIndexPageCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public string EmploymentStatusIds { get; set; }

        public string DepartmentIds { get; set; }

        public override string GetSettingKey() => "Employee_GetDataFilterForIndexPage";

        public override bool IsValid() => UserId > 0;
    }
}