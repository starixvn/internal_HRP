namespace HRP.Models.DataConfiguration.Department
{
    using System.Collections.Generic;
    using HRP.Models;

    public class DepartmentTableViewModel
    {
        public IEnumerable<Department> Departments { get; set; }

        public Field Field { get; set; }
    }
}