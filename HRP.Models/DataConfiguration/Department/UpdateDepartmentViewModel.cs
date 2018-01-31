namespace HRP.Models.DataConfiguration.Department
{
    using HRP.Models;

    public class UpdateDepartmentViewModel
    {
        public string FieldName { get; set; }

        public string LinkUpdateItem { get; set; }

        public Department Department { get; set; }
    }
}