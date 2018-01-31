namespace HRP.Models.DataConfiguration.Department
{
    using HRP.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateDepartmentViewModel
    {
        public CreateDepartmentViewModel()
        {
            Department = new Department();
        }

        public SelectList CompanyList { get; set; }

        public Department Department { get; set; }

        public string FieldName { get; set; }

        public string LinkCreateItem { get; set; }
    }
}