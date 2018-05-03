namespace HRP.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Services;
    using Fanex.Data.Repository;
    using Criteria.Report;
    using Models.Currency;
    using Models.MonthlyPayroll;
    using Models.Report;
    using NSubstitute;
    using Repositories;
    using Xunit;

    public class ReportServiceTests
    {
        private ICryptoService cryptoService;
        private IReportRepository reportRepository;
        private ISessionService sessionService;

        [Fact]
        public async Task GetDirectBankingBankInAsync_InValidParams_ReturnEmpty()
        {
            // Arrange
            var reportService = GetReportService();
            reportRepository.GetDataForBankInReportAsync(Arg.Any<ICriteria>()).Returns(new BankInReport());

            // Act
            var actual = await reportService.GetDirectBankingBankInAsync(0, DateTime.Now, PaymentType.Allowances, "");

            Assert.Null(actual.EBankingBankIns);
            Assert.Null(actual.CashBankIns);
            Assert.Null(actual.SelectedPaymentTransactions);
        }

        [Fact]
        public async Task GetEbankingBankInAsync_InValidParams_ReturnEmpty()
        {
            // Arrange
            var reportService = GetReportService();
            reportRepository.GetDataForBankInReportAsync(Arg.Any<ICriteria>()).Returns(new BankInReport());

            // Act
            var actual = await reportService.GetEbankingBankInAsync(0, DateTime.Now, "");

            Assert.Null(actual.EBankingBankIns);
            Assert.Null(actual.CashBankIns);
            Assert.Null(actual.SelectedPaymentTransactions);
        }

        [Fact]
        public async Task GetDirectBankingBankInAsync_ValidParams_ReturnListBankIns()
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
                SelectedPaymentTransactions = new List<string>(new[] { "EuroBank1" })
            };

            reportRepository.GetDataForBankInReportAsync(Arg.Any<ICriteria>()).Returns(bankInReport);

            cryptoService.Decrypt(Arg.Any<string>()).Returns("1");

            // Act
            var actual = await reportService.GetDirectBankingBankInAsync(1, DateTime.Now, PaymentType.BasicSalary, "1-1");

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

        [Fact]
        public async Task GetEbankingBankInAsync_ValidParams_ReturnListBankIns()
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
                SelectedPaymentTransactions = new List<string>(new[] { "EuroBank1" })
            };

            reportRepository.GetDataForBankInReportAsync(Arg.Any<ICriteria>()).Returns(bankInReport);

            cryptoService.Decrypt(Arg.Any<string>()).Returns("1");

            // Act
            var actual = await reportService.GetEbankingBankInAsync(1, DateTime.Now, "1-1");

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

        [Fact]
        public async Task GetDataForMonthlyPayrollReportAsync_WhenCalled_ReturnMonthlyPayrollReport()
        {
            // Arrange
            var report = new MonthlyPayrollReport
            {
                MonthlyBasicSalary = new List<MonthlyBasicSalaryModel> { new MonthlyBasicSalaryModel { EmployeeId = 3 } },
                MonthlySalary = new List<MonthlySalaryModel> { new MonthlySalaryModel { EmployeeId = 3 } },
                MonthlyBasicAllowance = new List<MonthlyPayrollBasicAllowanceModel> { new MonthlyPayrollBasicAllowanceModel { EmployeeId = 3 } },
                MonthlyAllowance = new List<MonthlyAllowanceModel> { new MonthlyAllowanceModel { EmployeeId = 3 } },
                CurrencyExchangeRate = new List<CurrencyExchangeRateModel>()
            };
            var month = DateTime.Today;
            var companyId = 21;
            var reportService = GetReportService();
            reportRepository.GetDataForMonthlyPayrollReportAsync(Arg.Is<GetDataForMonthlyPayrollReportCriteria>(x => x.CompanyId == companyId && x.Month == month))
                            .Returns(report);

            // Act
            var actual = await reportService.GetDataForMonthlyPayrollReportAsync(month, companyId);

            // Assert
            Assert.IsType<MonthlyPayrollReport>(actual);
            Assert.Equal(report, actual);
        }

        #region Helpers

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
            cryptoService = FakeCryptoService();
            reportRepository = FakeReportRepository();
            sessionService = FakeSessionService();

            return new ReportService(cryptoService, sessionService, reportRepository);
        }

        #endregion Helpers
    }
}