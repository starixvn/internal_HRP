namespace HRP.Models.EmployeeManagement
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EmployeeManagementViewModel
    {
        public bool IsHostCreateEmployee { get; set; }

        [Required(ErrorMessageResourceName = "SelectAtLeastOneOption", ErrorMessageResourceType = typeof(Message))]
        public string DepartmentIds { get; set; }

        public string EmployeeStatusIds { get; set; }

        public SelectList EmployeeStatus { get; set; }

        public int? EmployeeId { get; set; }
    }
}