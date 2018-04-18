namespace HRP.Models.EmployeeManagement
{
    using System.Collections.Generic;

    public class EmployeeManagementViewModel
    {

        public bool IsHostCreateEmployee { get; set; }

        public string DepartmentIds { get; set; }

        public string EmploymentStatusIds { get; set; }

        public int? EmployeeId { get; set; }

        public string DisplayName { get; set; }

        public IEnumerable<EmployeeBasicInfo> Employees { get; set; } = new List<EmployeeBasicInfo>();

    }
}