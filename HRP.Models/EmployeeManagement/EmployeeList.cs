namespace HRP.Models.EmployeeManagement
{
    public class EmployeeBasicInfo
    {
        public int EmployeeId { get; set; }

        public string IDNo => string.Format("{0}{1}", CompanyCode, EmployeeCode);

        public string EmployeeCode { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string CompanyCode { get; set; }

        public string CompanyName { get; set; }

        public string DivisionCode { get; set; }

        public string DivisionName { get; set; }

        public string DepartmentCode { get; set; }

        public string DepartmentName { get; set; }

        public string JobTitle { get; set; }

        public bool InService { get; set; }

        public bool InProbation { get; set; }

        public bool IsHostProfile { get; set; }

        public bool IsHostPayroll { get; set; }

        public bool IsHostBankDetails { get; set; }
    }
}