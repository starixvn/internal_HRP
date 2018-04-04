namespace HRP.Models.Payslip
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRP.Models.MonthlyPayroll;

    public class PayslipDetailModel
    {
        public int PayrollId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMMM - yyyy}")]
        public DateTime Month { get; set; }

        public string FullName { get; set; }
        public string IDNo { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public int SalaryCurrencyId { get; set; }
        public string SalaryCurrencyCode { get; set; }
        public string SalaryAmount { get; set; }
        public int AllowanceCurrencyId { get; set; }
        public string AllowanceCurrencyCode { get; set; }
        public string AllowanceAmount { get; set; }
        public string SalaryDeductionAmount { get; set; }
        public string AllowanceDeductionAmount { get; set; }
        public string GrossSalaryAmount { get; set; }
        public string GrossAllowanceAmount { get; set; }
        public string NetSalaryAmount { get; set; }
        public string NetAllowanceAmount { get; set; }
        public IEnumerable<MonthlySalaryAdditionModel> SalaryAdditions { get; set; } = new List<MonthlySalaryAdditionModel>();
        public IEnumerable<MonthlySalaryDeductionModel> SalaryDeductions { get; set; } = new List<MonthlySalaryDeductionModel>();
        public IEnumerable<MonthlyAllowanceAdditionModel> AllowanceAdditions { get; set; } = new List<MonthlyAllowanceAdditionModel>();
        public IEnumerable<MonthlyAllowanceDeductionModel> AllowanceDeductions { get; set; } = new List<MonthlyAllowanceDeductionModel>();
    }
}