namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Constants;
    using Core.Services;
    using Criteria;
    using Criteria.Report;
    using Models;
    using Models.Report;
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
            IReportRepository reportRepository = null)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
            this.cryptoService = cryptoService;
            this.reportRepository = reportRepository ?? new ReportRepository();
        }

        public async Task<BankInReport> GetBankInAsync(int companyId, DateTime selectedTime, PaymentType typeOfPayment, string selectedPaymentTransactions)
        {
            var criteria = new BankInReportCriteria
            {
                CompanyId = companyId,
                Month = selectedTime,
                PaymentTransactions = selectedPaymentTransactions,
                PaymentType = (int)typeOfPayment
            };

            var bankInData = await reportRepository.GetDataForBankInReportAsync(criteria).ConfigureAwait(false);

            ////var bankInReport = new BankInReport
            ////{
            ////    EBankingBankIns = await baseService.GetDataAsync<BankIn>(criteria).ConfigureAwait(false)
            ////};

            ////DecryptBankInAmount(bankInReport.EBankingBankIns);

            DecryptBankInAmount(bankInData.EBankingBankIns);
            ////DecryptBankInAmount(bankInData.DirectBanking1BankIns);
            ////DecryptBankInAmount(bankInData.DirectBanking2BankIns);
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
            return ColumnsConfirguration.Columns.Values.SelectMany(x => x).OrderBy(x => x.GroupName);
        }

        public async Task<MonthlyPayrollReport> GetDataForMonthlyPayrollReportAsync(DateTime month, int companyId)
        {
            var criteria = new GetDataForMonthlyPayrollReportCriteria
            {
                CompanyId = companyId,
                Month = month
            };

            var report = await reportRepository.GetDataForMonthlyPayrollReportAsync(criteria).ConfigureAwait(false);

            return report;
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