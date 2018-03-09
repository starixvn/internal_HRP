namespace HRP.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetEmployeeBasicInfoByIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Employee_GetBasicInfoById";

        public override bool IsValid() => EmployeeId > 0;
    }
}