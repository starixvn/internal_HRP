namespace HRP.Models.PaymentProcess
{
    using System.Collections.Generic;
    using HRP.Models.Currency;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.ServiceModels;

    public class GeneratePayrollModel
    {
        public IEnumerable<MonthlyPayroll> MonthlyPayroll { get; set; }

        public IEnumerable<MonthlyAttendance> MonthlyAttendance { get; set; }

        public IEnumerable<MonthlyDSTNetSalary> MonthlyDSTNetSalary { get; set; }

        public IEnumerable<MonthlyBasicSalaryModel> MonthlyBasicSalary { get; set; }

        public IEnumerable<MonthlyPayrollBasicAllowanceModel> MonthlyBasicAllowance { get; set; }

        public IEnumerable<CurrencyExchangeRateModel> CurrencyExchangeRate { get; set; }
        public IEnumerable<AdditionDeductionUploading> AdditionDeductions { get; set; } = new List<AdditionDeductionUploading>();
        public IEnumerable<Currency> Currencies { get; set; } = new List<Currency>();
    }
}