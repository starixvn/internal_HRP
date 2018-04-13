namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Models;
    using Models.Report;

    public interface IReportService
    {
        Task<IEnumerable<BankIn>> GetBankInAsync(int companyId, DateTime selectedTime);

        Task<IEnumerable<Company>> GetCompaniesAsync();

        IEnumerable<ColumnGroup> GetAvailableColumnsList();

        Task<IEnumerable<DynamicReportModel>> GetDataForDynamicReportAsync(GenerateReportViewModel viewModel);
    }
}