namespace HRP.Models
{
    using System.Collections.Generic;

    public class ListCompanyDivisionDepartment
    {
        public IEnumerable<Company> Companies { get; set; }

        public IEnumerable<Division> Divisions { get; set; }

        public IEnumerable<Department> Departments { get; set; }
    }
}