namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models.Report;

    public interface IReportService
    {
        Task<IEnumerable<BankIn>> GetBankInAsync(int companyId, DateTime selectedTime);

        Task<SelectList> GetCompanySelectListAsync();

        Task<IEnumerable<DynamicReportModel>> GetDataForDynamicReportAsync(GenerateReportViewModel viewModel);
    }
}