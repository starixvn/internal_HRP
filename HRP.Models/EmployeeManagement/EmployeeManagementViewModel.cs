namespace HRP.Models.EmployeeManagement
{
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EmployeeManagementViewModel
    {
        public bool IsHostCreateEmployee { get; set; }

        public string DepartmentIds { get; set; }

        public string EmployeeStatusIds { get; set; }

        public SelectList EmployeeStatus { get; set; }

        public int? EmployeeId { get; set; }
    }
}