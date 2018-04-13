namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Services;
    using Criteria.Report;
    using HRP.Core.Constants;
    using HRP.Criteria;
    using HRP.Models;
    using Models.Report;

    public class ReportService : IReportService
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;
        private readonly ICryptoService cryptoService;

        public ReportService(IBaseService baseService, ISessionService sessionService, ICryptoService cryptoService)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
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

        public async Task<IEnumerable<Company>> GetCompaniesAsync()
        {
            var criteria = new GetCompanyByFunctionKeyCriteria
            {
                UserId = sessionService.UserId,
                FunctionKey = FunctionKey.Report
            };
            var companies = await baseService.GetDataAsync<Company>(criteria).ConfigureAwait(false);

            return companies;
        }

        public IEnumerable<ColumnGroup> GetAvailableColumnsList()
        {
            return ColumnsDictionary.Columns.Values.OrderBy(x => x.GroupName);
        }

        public async Task<IEnumerable<DynamicReportModel>> GetDataForDynamicReportAsync(GenerateReportViewModel viewModel)
        {
            var criteria = new GetDataForDynamicReportCriteria
            {
                CompanyId = viewModel.CompanyId,
                Month = viewModel.SelectedTime,
                IsHasIDNo = viewModel.IsHasIDNo,
                IsHasFullName = viewModel.IsHasFullName,
                IsHasNickName = viewModel.IsHasNickName,
                IsHasGender = viewModel.IsHasGender,
                IsHasDateOfBirth = viewModel.IsHasDateOfBirth,
                IsHasPlaceOfBirth = viewModel.IsHasPlaceOfBirth,
                IsHasNationality = viewModel.IsHasNationality
            };

            var reports = await baseService.GetDataAsync<DynamicReportModel>(criteria).ConfigureAwait(false);

            return reports;
        }
    }
}