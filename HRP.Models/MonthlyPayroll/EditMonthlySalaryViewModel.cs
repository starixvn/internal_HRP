namespace HRP.Models.MonthlyPayroll
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRP.Models.Currency;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EditMonthlySalaryViewModel
    {
        public int PayrollId { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM-yyyy}")]
        public DateTime Month { get; set; }

        public string FullName { get; set; }

        public string Nickname { get; set; }

        public string IDNo { get; set; }

        public string CostCenter { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DueDate { get; set; }

        public int CompanyId { get; set; }

        public string CompanyCode { get; set; }

        public MonthlyBasicSalaryModel MonthlyBasicSalary { get; set; } = new MonthlyBasicSalaryModel();

        public MonthlySalaryModel MonthlySalary { get; set; } = new MonthlySalaryModel();

        public IList<MonthlySalaryAdditionModel> Additions { get; set; } = new List<MonthlySalaryAdditionModel>();

        public IList<MonthlySalaryDeductionModel> Deductions { get; set; } = new List<MonthlySalaryDeductionModel>();

        public SelectList Currencies { get; set; }

        public IEnumerable<CurrencyExchangeRateModel> CurrencyExchangeRates { get; set; } = new List<CurrencyExchangeRateModel>();

        public bool IsReset { get; set; }

        public bool IsHost { get; set; }
    }
}