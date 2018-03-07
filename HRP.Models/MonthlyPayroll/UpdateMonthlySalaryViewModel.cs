namespace HRP.Models.MonthlyPayroll
{
    using System;
    using System.Collections.Generic;

    public class UpdateMonthlySalaryViewModel
    {
        public long PayrollId { get; set; }
        public DateTime Month { get; set; }
        public int CompanyId { get; set; }
        public string CompanyCode { get; set; }
        public MonthlyBasicSalaryViewModel MonthlyBasicSalary { get; set; }
        public MonthlySalaryViewModel MonthlySalary { get; set; }
        public IEnumerable<MonthlySalaryAdditionViewModel> Additions { get; set; }
        public IEnumerable<MonthlySalaryDeductionViewModel> Deductions { get; set; }
    }
}