namespace HRP.Models.MonthlyPayroll
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRP.Models.Currency;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EditMonthlySalaryViewModel : RijndaelData
    {
        [DisplayFormat(DataFormatString = "{0:MM-yyyy}")]
        public DateTime Month { get; set; }

        public MonthlyPayrollFilterInfoModel FilterInfo { get; set; } = new MonthlyPayrollFilterInfoModel();
        public MonthlyBasicSalaryViewModel MonthlyBasicSalary { get; set; } = new MonthlyBasicSalaryViewModel();
        public MonthlySalaryViewModel MonthlySalary { get; set; } = new MonthlySalaryViewModel();
        public IList<MonthlySalaryAdditionViewModel> Additions { get; set; } = new List<MonthlySalaryAdditionViewModel>();
        public IList<MonthlySalaryDeductionViewModel> Deductions { get; set; } = new List<MonthlySalaryDeductionViewModel>();
        public SelectList Currencies { get; set; }
        public SelectList EBankingAccounts { get; set; }
        public SelectList DirectBankingAccounts { get; set; }
        public SelectList CashAccounts { get; set; }
        public IEnumerable<CurrencyExchangeRateModel> CurrencyExchangeRates { get; set; } = new List<CurrencyExchangeRateModel>();
        public bool IsRedirectToEditAllowance { get; set; }
        public CachingFilterViewModel CachingFilter { get; set; } = new CachingFilterViewModel();
        public int VNDCurrencyId { get; set; }
    }
}