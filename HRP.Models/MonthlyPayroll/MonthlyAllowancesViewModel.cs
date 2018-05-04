namespace HRP.Models.MonthlyPayroll
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRP.Models.Currency;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class MonthlyAllowancesViewModel
    {
        public int EmployeeId { get; set; }

        public int CompanyId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-yyyy}")]
        public DateTime Month { get; set; }

        public MonthlyAllowanceModel Allowance { get; set; }

        public MonthlyPayrollBasicAllowanceModel BasicAllowance { get; set; }

        public IList<MonthlyAllowanceAdditionModel> Additions { get; set; }

        public IList<MonthlyAllowanceDeductionModel> Deductions { get; set; }

        public MonthlyPayrollFilterInfoModel EmployeeInfo { get; set; }

        public SelectList Currencies { get; set; }

        public IEnumerable<CurrencyExchangeRateModel> CurrencyExchangeRates { get; set; }

        public SelectList CashAccounts { get; set; }

        public bool CanEdit { get; set; }

        public CachingFilterViewModel Filter { get; set; }

        public int DaysInMonth { get; set; }

        public int VNDCurrencyId { get; set; }
    }
}