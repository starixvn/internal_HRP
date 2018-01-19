namespace HRP.Models.EmployeeManagement
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Core.Enumerations;
    using HRP.Resources;

    public class EmployeeManagementViewModel
    {
        public bool IsHostCreateEmployee { get; set; }

        [Required(ErrorMessageResourceName = "SelectAtLeastOneOption", ErrorMessageResourceType = typeof(Message))]
        public string DepartmentIds { get; set; }

        public Selection InService { get; set; }

        public Selection InProbation { get; set; }

        public int EmployeeId { get; set; }
    }
}