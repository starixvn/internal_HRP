namespace HRP.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Services;
    using Fanex.Data.Repository;
    using Models.Report;
    using NSubstitute;
    using Xunit;

    public class ReportServiceTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private ICryptoService cryptoService;

        [Fact]
        public async Task GetBankInAsync_InValidParams_ReturnEmpty()
        {
            // Arrange
            var reportService = GetReportService();

            // Act
            var actual = await reportService.GetBankInAsync(0, DateTime.Now);

            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetBankInAsync_ValidParams_ReturnListBankIns()
        {
            // Arrange
            var reportService = GetReportService();

            float salaryDescrypted = 1;

            var bankInData = new List<BankIn>
            {
                new BankIn { Salary = "1" },
                new BankIn { Salary = "1" },
            };

            baseService.GetDataAsync<BankIn>(Arg.Any<ICriteria>()).Returns(bankInData);
            cryptoService.Decrypt(Arg.Any<string>()).Returns("1");

            // Act
            var actual = await reportService.GetBankInAsync(1, DateTime.Now);

            Assert.All(actual, x =>
            {
                Assert.Equal(salaryDescrypted, x.SalaryDecrypted);
            });
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

        private ReportService GetReportService()
        {
            baseService = FakeBaseService();
            cryptoService = FakeCryptoService();
            sessionService = FakeSessionService();

            return new ReportService(baseService, sessionService, cryptoService);
        }

        #endregion Helpers
    }
}