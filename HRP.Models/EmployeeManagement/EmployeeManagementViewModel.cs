namespace HRP.Models.EmployeeManagement
{
    using HRP.Core.Enumerations;

    public class EmployeeManagementViewModel
    {
        public string DepartmentIds { get; set; }

        public Selection InService { get; set; }

        public Selection InProbation { get; set; }

        public int EmployeeId { get; set; }
    }
}