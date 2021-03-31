namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Enumerations;
    using Models.Report;

    public interface IReportService
    {
        Task<DirectBankingReport> GetDirectBankingBankInAsync(
            int companyId,
            DateTime selectedTime,
            PaymentType typeOfPayment,
            string selectedPaymentTransactions);

        Task<DirectBankingReport> GetEbankingBankInAsync(
            int companyId,
            DateTime selectedTime,
            string selectedPaymentTransactions);

        Task<MonthlyPayrollReport> GetDataForMonthlyPayrollReportAsync(DateTime month, int companyId, int currencyId);

        Task<IEnumerable<DynamicReportModel>> GetEmployeeDataForDynamicReport(int companyId, string employeeStatusIds);

        Task<string> GetUserFilePasswordAsync(int userId);

        Task EditUserFilePasswordAsync(int userId, string password);
    }
}