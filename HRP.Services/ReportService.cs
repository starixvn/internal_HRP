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
        private readonly ICryptoService cryptoService;

        public ReportService(IBaseService baseService, ICryptoService cryptoService)
        {
            this.baseService = baseService;
            this.cryptoService = cryptoService;
        }

        public async Task<IEnumerable<BankIn>> GetBankInAsync(int companyId, DateTime selectedTime)
        {
            var criteria = new BankInReportCriteria
            {
                CompanyId = companyId,
                Month = selectedTime
            };

            var bankInData = await baseService.GetDataAsync<BankIn>(criteria).ConfigureAwait(false);

            foreach (BankIn bankIn in bankInData)
            {
                bankIn.SalaryDecrypted = float.Parse(cryptoService.Decrypt(bankIn.Salary));
            }

            return bankInData;
        }
    }
}
