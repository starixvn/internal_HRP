namespace HRP.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetAllCompanyDivsionDepartmentCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override string GetSettingKey() => "Employee_GetAllCompanyDivisionDepartment";

        public override bool IsValid() => UserId > 0;
    }
}