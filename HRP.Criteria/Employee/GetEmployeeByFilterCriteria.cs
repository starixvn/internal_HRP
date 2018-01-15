namespace HRP.Criteria.Employee
{
    using Fanex.Data.Repository;
    using HRP.Core.Enumerations;

    public class GetAllEmployeeCriteria : CriteriaBase
    {
        public string CompanyIds { get; set; }

        public string DepartmentIds { get; set; }

        public string DivisionIds { get; set; }

        public string SectionIds { get; set; }

        public Selection InService { get; set; }

        public Selection InProbation { get; set; }

        public int? EmployeeId { get; set; }

        public override string GetSettingKey() => "Employee_GetByFiler";

        public override bool IsValid() => true;
    }
}