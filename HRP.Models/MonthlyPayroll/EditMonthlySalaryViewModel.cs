namespace HRP.Models.MonthlyPayroll
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRP.Models.Currency;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EditMonthlySalaryViewModel
    {
        [DisplayFormat(DataFormatString = "{0:MM-yyyy}")]
        public DateTime Month { get; set; }

        public MonthlyPayrollFilterInfoModel FilterInfo { get; set; } = new MonthlyPayrollFilterInfoModel();

        public MonthlyBasicSalaryModel MonthlyBasicSalary { get; set; } = new MonthlyBasicSalaryModel();

        public MonthlySalaryModel MonthlySalary { get; set; } = new MonthlySalaryModel();

        public IList<MonthlySalaryAdditionModel> Additions { get; set; } = new List<MonthlySalaryAdditionModel>();

        public IList<MonthlySalaryDeductionModel> Deductions { get; set; } = new List<MonthlySalaryDeductionModel>();

        public SelectList Currencies { get; set; }

        public SelectList CashAccounts { get; set; }

        public IEnumerable<CurrencyExchangeRateModel> CurrencyExchangeRates { get; set; } = new List<CurrencyExchangeRateModel>();

        public bool IsRedirectToEditAllowance { get; set; }

        public CachingFilterViewModel CachingFilter { get; set; } = new CachingFilterViewModel();
    }
}