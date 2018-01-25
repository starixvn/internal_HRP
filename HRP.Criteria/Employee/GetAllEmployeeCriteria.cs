namespace HRP.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetAllEmployeeCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public string DepartmentIds { get; set; }

        public string EmployeeStatusIds { get; set; }

        public int? EmployeeId { get; set; }

        public string EmployeeName { get; set; } = string.Empty;

        public override string GetSettingKey() => "Employee_GetAllEmployees";

        public override bool IsValid() => UserId > 0;
    }
}