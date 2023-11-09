namespace HRP.Models.DataConfiguration.Department
{
    using HRP.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateDepartmentViewModel
    {
        public SelectList CompanyList { get; set; }

        public Department Department { get; set; } = new Department();

        public Field Field { get; set; }
    }
}