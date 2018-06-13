namespace HRP.Models.MonthlyPayroll
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRP.Models.Currency;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EditMonthlyAllowancesViewModel
    {
        public int EmployeeId { get; set; }

        public int CompanyId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-yyyy}")]
        public DateTime Month { get; set; }

        public MonthlyAllowanceViewModel Allowance { get; set; } = new MonthlyAllowanceViewModel();

        public MonthlyPayrollBasicAllowanceViewModel BasicAllowance { get; set; } = new MonthlyPayrollBasicAllowanceViewModel();

        public IList<MonthlyAllowanceDetailAdjustmentViewModel> Additions { get; set; } = new List<MonthlyAllowanceDetailAdjustmentViewModel>();

        public IList<MonthlyAllowanceDetailAdjustmentViewModel> Deductions { get; set; } = new List<MonthlyAllowanceDetailAdjustmentViewModel>();

        public MonthlyPayrollFilterInfoModel EmployeeInfo { get; set; } = new MonthlyPayrollFilterInfoModel();

        public SelectList Currencies { get; set; }

        public IEnumerable<CurrencyExchangeRateModel> CurrencyExchangeRates { get; set; }

        public SelectList CashAccounts { get; set; }

        public bool CanEdit { get; set; }

        public CachingFilterViewModel Filter { get; set; } = new CachingFilterViewModel();

        public int DaysInMonth { get; set; }

        public int VNDCurrencyId { get; set; }

        public string RijndaelIV { get; set; }

        public string RijndaelKey { get; set; }
    }
}