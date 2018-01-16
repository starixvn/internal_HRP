namespace HRP.Criteria.Employee
{
    using Fanex.Data.Repository;
    using HRP.Core.Enumerations;

    public class GetAllEmployeeCriteria : CriteriaBase
    {
        public string DepartmentIds { get; set; }

        public Selection InService { get; set; }

        public Selection InProbation { get; set; }

        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Employee_GetAllEmployees";

        public override bool IsValid() => true;
    }
}