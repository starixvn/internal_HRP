namespace HRP.Models.MonthlyPayroll
{
    using System;
    using System.Collections.Generic;

    public class EditMonthlySalaryViewModel
    {
        public long PayrollId { get; set; }
        public DateTime Month { get; set; }
        public int CompanyId { get; set; }
        public string CompanyCode { get; set; }
        public int WorkingDays { get; set; }
        public MonthlyBasicSalaryViewModel MonthlyBasicSalary { get; set; } = new MonthlyBasicSalaryViewModel();
        public MonthlySalaryViewModel MonthlySalary { get; set; } = new MonthlySalaryViewModel();
        public IEnumerable<MonthlySalaryAdditionViewModel> Additions { get; set; } = new List<MonthlySalaryAdditionViewModel>();
        public IEnumerable<MonthlySalaryDeductionViewModel> Deductions { get; set; } = new List<MonthlySalaryDeductionViewModel>();
    }
}