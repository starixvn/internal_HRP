namespace HRP.Models.DataConfiguration.Department
{
    using System.Collections.Generic;
    using HRP.Models;

    public class DepartmentTableViewModel
    {
        public IEnumerable<Department> Departments { get; set; }

        public string FieldName { get; set; }

        public string LinkCreateItem { get; set; }

        public string LinkUpdateItem { get; set; }
    }
}