namespace HRP.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetEmployeeByIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Employee_GetByEmployeeId";

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}