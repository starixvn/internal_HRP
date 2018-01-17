namespace HRP.Models
{
    using System;

    [Serializable]
    public class Department
    {
        public int DepartmentId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int DivisionId { get; set; }
    }
}