namespace HRP.Models.EmployeeManagement
{
    using System;

    public class WorkPermit
    {
        public int EmployeeId { get; set; }

        public int CompanyId { get; set; }

        public int Type { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}