namespace HRP.Models.EmployeeManagement
{
    using System;
    using HRP.Core.Enumerations;

    [Serializable]
    public class EmployeeBasicInfo
    {
        public int EmployeeId { get; set; }

        public string IDNo => string.Format("{0}{1}", CompanyCode, EmployeeCode);

        public string EmployeeCode { get; set; }

        public string Nickname { get; set; }

        public string FullName { get; set; }

        public int CompanyId { get; set; }

        public string CompanyCode { get; set; }

        public string DivisionCode { get; set; }

        public string DepartmentCode { get; set; }

        public string CostCenterCode { get; set; }

        public string SectionCode { get; set; }

        public string JobTitle { get; set; }

        public string EmployeeStatus { get; set; }

        public Policy EditProfilePolicy { get; set; }

        public Policy EditPayRollPolicy { get; set; }

        public Policy EditBankPolicy { get; set; }
    }
}