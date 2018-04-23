namespace HRP.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Services;
    using Fanex.Data.Repository;
    using Models.Report;
    using NSubstitute;
    using Repositories;
    using Xunit;

    public class ReportServiceTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private ICryptoService cryptoService;
        private IReportRepository reportRepository;

        [Fact]
        public async Task GetBankInAsync_InValidParams_ReturnEmpty()
        {
            // Arrange
            var reportService = GetReportService();
            reportRepository.GetDataForBankInReportAsync(Arg.Any<ICriteria>()).Returns(new BankInReport());

            // Act
            var actual = await reportService.GetBankInAsync(0, DateTime.Now, PaymentType.Allowances, "");

            Assert.Null(actual.EBankingBankIns);
            Assert.Null(actual.CashBankIns);
            Assert.Null(actual.SelectedPaymentTransactions);
        }

        [Fact]
        public async Task GetBankInAsync_ValidParams_ReturnListBankIns()
        {
            // Arrange
            var reportService = GetReportService();

            float salaryDescrypted = 1;
            var companyName = "Dashword";
            var reportedMonth = DateTime.Today;

            var bankInData = new List<BankIn>
            {
                new BankIn { Amount = "1" },
                new BankIn { Amount = "1" },
            };

            var bankInReport = new BankInReport
            {
                EBankingBankIns = bankInData,
                CashBankIns = bankInData,
                CompanyName = companyName,
                ReportedMonth = reportedMonth,
                SelectedPaymentTransactions = new List<string>(new[] {"EuroBank1"})
            };

            reportRepository.GetDataForBankInReportAsync(Arg.Any<ICriteria>()).Returns(bankInReport);

            cryptoService.Decrypt(Arg.Any<string>()).Returns("1");

            // Act
            var actual = await reportService.GetBankInAsync(1, DateTime.Now, PaymentType.BasicSalary, "1-1");

            Assert.All(actual.EBankingBankIns, x =>
            {
                Assert.Equal(salaryDescrypted, x.AmountDecrypted);
            });
            Assert.All(actual.CashBankIns, x =>
            {
                Assert.Equal(salaryDescrypted, x.AmountDecrypted);
            });
            Assert.Equal(companyName, actual.CompanyName);
            Assert.Equal(reportedMonth, actual.ReportedMonth);
            Assert.Equal(bankInReport.SelectedPaymentTransactions, actual.SelectedPaymentTransactions);
        }

        #region Helpers

        private static IBaseService FakeBaseService()
        {
            return Substitute.For<IBaseService>();
        }

        private static ICryptoService FakeCryptoService()
        {
            return Substitute.For<ICryptoService>();
        }

        private static ISessionService FakeSessionService()
        {
            return Substitute.For<ISessionService>();
        }

        private static IReportRepository FakeReportRepository()
        {
            return Substitute.For<IReportRepository>();
        }

        private ReportService GetReportService()
        {
            baseService = FakeBaseService();
            cryptoService = FakeCryptoService();
            sessionService = FakeSessionService();
            reportRepository = FakeReportRepository();

            return new ReportService(baseService, sessionService, cryptoService, reportRepository);
        }

        #endregion Helpers
    }
}