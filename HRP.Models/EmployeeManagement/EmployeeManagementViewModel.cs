namespace HRP.Models.EmployeeManagement
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EmployeeManagementViewModel
    {
        public bool IsHostCreateEmployee { get; set; }

        public string DepartmentIds { get; set; }

        public string EmployeeStatusIds { get; set; }

        public SelectList EmployeeStatus { get; set; }

        public int? EmployeeId { get; set; }

        public string DisplayName { get; set; }

        public IEnumerable<EmployeeBasicInfo> Employees { get; set; }
    }
}