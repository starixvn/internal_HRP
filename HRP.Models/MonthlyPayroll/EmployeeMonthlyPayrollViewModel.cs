namespace HRP.Models.MonthlyPayroll
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class EmployeeMonthlyPayrollViewModel
    {
        [DisplayFormat(DataFormatString = "{0:MM-yyyy}")]
        public DateTime Month { get; set; }

        public int CompanyId { get; set; }

        public int? EmployeeId { get; set; }

        public string Employee { get; set; }

        public string IDNo { get; set; }

        public string CostCenterIds { get; set; }

        public int? WorkingDays { get; set; }

        public MonthlyPayrollFilterInfoModel FilterInfo { get; set; } = new MonthlyPayrollFilterInfoModel();

        public IEnumerable<MonthlyPayrollEmployeeInfoViewModel> Employees { get; set; } = new List<MonthlyPayrollEmployeeInfoViewModel>();
    }
}