namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Core.Services;
    using Criteria.Report;
    using Models.Report;

    [ExcludeFromCodeCoverage]
    public class ReportService : IReportService
    {
        private readonly IBaseService baseService;

        public ReportService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<BankIn>> GetBankInAsync(int companyId, DateTime selectedTime)
        {
            var criteria = new BankInReportCriteria
            {
                CompanyId = companyId,
                Month = selectedTime
            };

            return await baseService.GetDataAsync<BankIn>(criteria).ConfigureAwait(false);
        }
    }
}
