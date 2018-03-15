namespace HRP.Models.MonthlyPayroll
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class EditMonthlySalaryViewModel
    {
        public int PayrollId { get; set; }
        public DateTime Month { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DueDate { get; set; }

        public int CompanyId { get; set; }
        public string CompanyCode { get; set; }
        public int WorkingDays { get; set; }
        public MonthlyBasicSalaryModel MonthlyBasicSalary { get; set; } = new MonthlyBasicSalaryModel();
        public MonthlySalaryModel MonthlySalary { get; set; } = new MonthlySalaryModel();
        public IEnumerable<MonthlySalaryAdditionModel> Additions { get; set; } = new List<MonthlySalaryAdditionModel>();
        public IEnumerable<MonthlySalaryDeductionModel> Deductions { get; set; } = new List<MonthlySalaryDeductionModel>();
        public bool IsReset { get; set; }
        public bool IsHost { get; set; }
    }
}