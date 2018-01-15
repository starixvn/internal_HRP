namespace HRP.Models.EmployeeManagement
{
    public class EmployeeList
    {
        public int EmployeeId { get; set; }

        public string IDNo => string.Format("{0}{1}", CompanyCode, EmployeeCode);

        public string CompanyCode { get; set; }

        public string EmployeeCode { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Company { get; set; }

        public string Division { get; set; }

        public string Department { get; set; }

        public string JobTitle { get; set; }

        public bool InService { get; set; }

        public bool InProbation { get; set; }
    }
}