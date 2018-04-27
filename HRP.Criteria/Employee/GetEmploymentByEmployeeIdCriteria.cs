namespace HRP.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetEmploymentByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Employment_GetByEmployeeId";

        public override bool IsValid() => EmployeeId > 0;
    }
}