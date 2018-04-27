namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Services;
    using Criteria.Report;
    using Models.Report;
    using Repositories;

    public class ReportService : IReportService
    {
        private readonly ICryptoService cryptoService;
        private readonly IReportRepository reportRepository;

        public ReportService(
            ICryptoService cryptoService,
            IReportRepository reportRepository = null)
        {
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

            DecryptBankInAmount(bankInData.EBankingBankIns);
            DecryptBankInAmount(bankInData.CashBankIns);

            return bankInData;
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