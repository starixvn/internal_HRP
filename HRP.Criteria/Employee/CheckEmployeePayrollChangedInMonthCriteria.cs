namespace HRP.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    public class CheckEmployeePayrollChangedInMonthCriteria : CriteriaBase
    {
        public DateTime Month { get; set; }

        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Employee_CheckEmployeePayrollChangedInMonth";

        public override bool IsValid() => EmployeeId > 0;
    }
}