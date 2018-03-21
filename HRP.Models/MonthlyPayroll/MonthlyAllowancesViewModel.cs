namespace HRP.Models.MonthlyPayroll
{
    using System;
    using System.Collections.Generic;
    using HRP.Models.Currency;

    public class MonthlyAllowancesViewModel
    {
        public int CompanyId { get; set; }

        public DateTime Month { get; set; }

        public MonthlyAllowanceModel Allowance { get; set; }

        public MonthlyPayrollBasicAllowanceModel BasicAllowance { get; set; }

        public IEnumerable<MonthlyAllowanceAdditionModel> Additions { get; set; }

        public IEnumerable<MonthlyAllowanceDeductionModel> Deductions { get; set; }

        public IEnumerable<CurrencyCodeModel> Currencies { get; set; }

        public IEnumerable<CurrencyExchangeRateModel> CurrencyExchangeRates { get; set; }

        public bool CanEdit { get; set; }
    }
}