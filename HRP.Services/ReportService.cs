namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Constants;
    using Core.Services;
    using Criteria.Report;
    using Criteria;
    using Models.Report;
    using Models;
    using Repositories;

    public class ReportService : IReportService
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;
        private readonly ICryptoService cryptoService;
        private readonly IReportRepository reportRepository;

        public ReportService(
            IBaseService baseService,
            ISessionService sessionService,
            ICryptoService cryptoService,
            IReportRepository reportRepository)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
            this.cryptoService = cryptoService;
            this.reportRepository = reportRepository;
        }

        public async Task<BankInReport> GetBankInAsync(int companyId, DateTime selectedTime)
        {
            var criteria = new BankInReportCriteria
            {
                CompanyId = companyId,
                Month = selectedTime
            };

            var bankInData = await reportRepository.GetDataForBankInReportAsync(criteria).ConfigureAwait(false);

            DecryptBankInAmount(bankInData.EBankingBankIns);
            DecryptBankInAmount(bankInData.DirectBanking1BankIns);
            DecryptBankInAmount(bankInData.DirectBanking2BankIns);
            DecryptBankInAmount(bankInData.CashBankIns);

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

        public async Task<IEnumerable<DynamicReportModel>> GetDataForDynamicReportAsync(
            GenerateReportViewModel viewModel)
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

        private void DecryptBankInAmount(IEnumerable<BankIn> bankIns)
        {
            if (bankIns != null && bankIns.Any())
            {
                foreach (BankIn bankIn in bankIns)
                {
                    float parsedValue;
                    float.TryParse(cryptoService.Decrypt(bankIn.Amount), out parsedValue);
                    bankIn.AmountDecrypted = parsedValue;
                }
            }
        }
    }
}