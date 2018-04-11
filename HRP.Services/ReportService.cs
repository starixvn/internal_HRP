namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Threading.Tasks;
    using Core.Services;
    using Criteria.Report;
    using Interfaces;
    using Models.Report;

    [ExcludeFromCodeCoverage]
    public class ReportService : IReportService
    {
        private readonly IBaseService baseService;
        private readonly IExportExcelService exportExcelService;
        private readonly ICryptoService cryptoService;

        public ReportService(IBaseService baseService, IExportExcelService exportExcelService, ICryptoService cryptoService)
        {
            this.baseService = baseService;
            this.exportExcelService = exportExcelService;
            this.cryptoService = cryptoService;
        }

        private async Task<IEnumerable<BankIn>> GetBankInAsync(int companyId, DateTime selectedTime)
        {
            var criteria = new BankInReportCriteria
            {
                CompanyId = companyId,
                Month = selectedTime
            };

            return await baseService.GetDataAsync<BankIn>(criteria).ConfigureAwait(false);
        }

        public async Task<byte[]> ExportBankInAsync(int companyId, DateTime selectedTime, string templateFileString)
        {
            var bankInData = await GetBankInAsync(companyId, selectedTime).ConfigureAwait(false);

            foreach (BankIn bankIn in bankInData)
            {
                bankIn.Salary = cryptoService.Decrypt(bankIn.Salary);
            }

            FileInfo fileInfo = new FileInfo(templateFileString);

            return exportExcelService.ExportBankInReports(companyId, selectedTime, fileInfo, bankInData);
        }
    }
}
