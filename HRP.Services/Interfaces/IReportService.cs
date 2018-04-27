namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.Report;

    public interface IReportService
    {
        Task<BankInReport> GetBankInAsync(int companyId, DateTime selectedTime, PaymentType typeOfPayment, string selectedPaymentTransactions);

        IEnumerable<ColumnGroup> GetAvailableColumnsList();

        Task<MonthlyPayrollReport> GetDataForMonthlyPayrollReportAsync(DateTime month, int companyId);
    }
}