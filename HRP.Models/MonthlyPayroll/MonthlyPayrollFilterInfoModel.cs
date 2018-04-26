namespace HRP.Models.MonthlyPayroll
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class MonthlyPayrollFilterInfoModel
    {
        public int PayrollId { get; set; }
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Nickname { get; set; }
        public string IdNo { get; set; }
        public string CostCenter { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DueDate { get; set; }

        public int CompanyId { get; set; }
        public string CompanyCode { get; set; }
        public int WorkingDays { get; set; }
        public bool IsHost { get; set; }

        public int TotalEmployees { get; set; }
    }
}