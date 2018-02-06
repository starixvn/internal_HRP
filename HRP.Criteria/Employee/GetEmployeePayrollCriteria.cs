namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetEmployeePayrollCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int UserId { get; set; }

        public override string GetSettingKey() => "Employee_GetPayroll";

        public override bool IsValid() => EmployeeId > 0 && UserId > 0;
    }
}