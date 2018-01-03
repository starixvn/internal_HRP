namespace HRP.Criteria.EmployeeManagement
{
    using System;

    public class CreateEmployeeCriteria : CommandBaseCriteria
    {
        public int EmployeeId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public override string GetSettingKey() => "Employee_CreateEmployee";

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(FullName) && !string.IsNullOrEmpty(NickName);
        }
    }
}