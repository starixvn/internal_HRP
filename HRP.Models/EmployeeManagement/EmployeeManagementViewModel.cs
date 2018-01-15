using HRP.Core.Enumerations;

namespace HRP.Models.EmployeeManagement
{
    public class EmployeeManagementViewModel
    {
        public Selection InService { get; set; }

        public Selection InProbation { get; set; }

        public int? EmployeeId { get; set; }
    }
}