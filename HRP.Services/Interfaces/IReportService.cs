namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Criteria.Report;
    using Models.Report;

    public interface IReportService
    {
        Task<DirectBankingReport> GetDirectBankingBankInAsync(GetDataForDirectBankingReportCriteria criteria);

        Task<DirectBankingReport> GetEbankingBankInAsync(GetDataForEbankingReportCriteria criteria);

        Task<MonthlyPayrollReport> GetDataForMonthlyPayrollReportAsync(DateTime month, int companyId, int currencyId);

        Task<IEnumerable<DynamicReportModel>> GetEmployeeDataForDynamicReport(int companyId, string employeeStatusIds);

        Task<string> GetUserFilePasswordAsync(int userId);

        Task EditUserFilePasswordAsync(int userId, string password);
    }
}