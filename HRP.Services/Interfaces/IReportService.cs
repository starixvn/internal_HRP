namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Models;
    using Models.Report;

    public interface IReportService
    {
        Task<BankInReport> GetBankInAsync(int companyId, DateTime selectedTime, PaymentType typeOfPayment, string selectedPaymentTransactions);

        Task<IEnumerable<Company>> GetCompaniesAsync();

        IEnumerable<ColumnGroup> GetAvailableColumnsList();

        Task<MonthlyPayrollReport> GetDataForMonthlyPayrollReportAsync(DateTime month, int companyId);
    }
}