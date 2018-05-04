namespace HRP.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Services;
    using HRP.Core.Enumerations;
    using HRP.Core.Model;
    using HRP.Criteria.Payslip;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.Payslip;
    using HRP.Repositories;
    using NSubstitute;
    using Xunit;

    public class PayslipServiceTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private ICryptoService cryptoService;
        private IBaseMailService emailService;
        private IViewRenderService viewRenderService;
        private IPdfRenderService pdfRenderService;
        private IPayslipRepository payslipRepository;

        [Fact]
        public async Task GetPayslipAsync_WhenCalled_ReturnsAsExpectedPayslips()
        {
            // Arrange
            var payslips = new List<PayslipModel>
            {
                new PayslipModel { PayrollId = 1, IDNo = "DAR111", FullName = "thang", Nickname = "tim.trinh",
                    Section = "HR", Email = "tim.trinh@starixsoft.com", Phone = "123", IsSent = true, PayrollStatus = MonthlyPayrollStatus.Default }
            };
            var payslipService = GetPayslipService();
            baseService.GetDataAsync<PayslipModel>(Arg.Any<GetPayslipCriteria>()).Returns(payslips);

            // Act
            var actual = await payslipService.GetPayslipAsync(Arg.Any<DateTime>(), Arg.Any<int>(), Arg.Any<bool?>());

            // Assert
            Assert.Equal(actual, payslips);
        }

        #region GetPayslipDetailAsync

        [Fact]
        public async Task GetPayslipDetailAsync_NotExistedPayroll_ReturnDefaultModel()
        {
            // Arrange
            var payslips = new List<PayslipDetailModel>();
            var payslipService = GetPayslipService();
            payslipRepository.GetPayslipDetailsAsync(Arg.Any<GetPayslipDetailsCriteria>()).Returns(payslips);

            // Act
            var actual = await payslipService.GetPayslipDetailAsync(1, 1);

            // Assert
            Assert.Null(actual);
        }

        [Theory]
        [InlineData(11, "vnd", "1,000.00")]
        [InlineData(1, "eur", "1,234.00")]
        public async Task GetPayslipDetailAsync_ExistedPayroll_ReturnsAsExpectedModel(int currencyId, string currencyCode, string expectedAmount)
        {
            // Arrange
            var plainText = "1234";
            var payslips = new List<PayslipDetailModel>
            {
                new PayslipDetailModel
                {
                    SalaryCurrencyId = currencyId,
                    SalaryCurrencyCode = currencyCode,
                    AllowanceCurrencyId = currencyId,
                    AllowanceCurrencyCode = currencyCode,
                    SalaryAdditions = new List<MonthlySalaryAdditionModel> { new MonthlySalaryAdditionModel() },
                    SalaryDeductions = new List<MonthlySalaryDeductionModel> { new MonthlySalaryDeductionModel() },
                    AllowanceAdditions = new List<MonthlyAllowanceAdditionModel> { new MonthlyAllowanceAdditionModel() },
                    AllowanceDeductions = new List<MonthlyAllowanceDeductionModel> { new MonthlyAllowanceDeductionModel() }
                }
            };
            var payslipService = GetPayslipService();
            payslipRepository.GetPayslipDetailsAsync(Arg.Any<GetPayslipDetailsCriteria>()).Returns(payslips);
            cryptoService.Decrypt(Arg.Any<string>()).Returns(plainText);

            // Act
            var actual = await payslipService.GetPayslipDetailAsync(1, 1);

            // Assert
            Assert.Equal(payslips.First().PayrollId, actual.PayrollId);
            Assert.Equal(payslips.First().Month, actual.Month);
            Assert.Equal(payslips.First().FullName, actual.FullName);
            Assert.Equal(payslips.First().IDNo, actual.IDNo);
            Assert.Equal(payslips.First().CompanyName, actual.CompanyName);
            Assert.Equal(payslips.First().SalaryCurrencyId, actual.SalaryCurrencyId);
            Assert.Equal(payslips.First().SalaryCurrencyCode, actual.SalaryCurrencyCode);
            Assert.Equal(expectedAmount, actual.SalaryAmount);
            Assert.Equal(payslips.First().AllowanceCurrencyId, actual.AllowanceCurrencyId);
            Assert.Equal(payslips.First().AllowanceCurrencyCode, actual.AllowanceCurrencyCode);
            Assert.Equal(expectedAmount, actual.AllowanceAmount);
            Assert.Collection(
                actual.SalaryAdditions,
                kvp => { Assert.Equal(plainText, kvp.NetSalary); Assert.Equal(expectedAmount, kvp.NetSalaryDisplay); });

            Assert.Collection(
                actual.SalaryDeductions,
                kvp => { Assert.Equal(plainText, kvp.NetSalary); Assert.Equal(expectedAmount, kvp.NetSalaryDisplay); });

            Assert.Collection(
                actual.AllowanceAdditions,
                kvp => { Assert.Equal(plainText, kvp.NetAllowance); Assert.Equal(expectedAmount, kvp.NetAllowanceToDisplay); });

            Assert.Collection(
                actual.AllowanceDeductions,
                kvp => { Assert.Equal(plainText, kvp.NetAllowance); Assert.Equal(expectedAmount, kvp.NetAllowanceToDisplay); });
        }

        #endregion GetPayslipDetailAsync

        [Fact]
        public async Task SendPayslipAsync_WhenCalled_ExpectedSendMail()
        {
            // Arrange
            var viewModel = new SendPayslipViewModel();
            var payslips = new List<PayslipDetailModel>
            {
                new PayslipDetailModel { PayrollId = 1 },
                new PayslipDetailModel { PayrollId = 2 }
            };
            var payslipService = GetPayslipService();
            payslipRepository.GetPayslipDetailsAsync(Arg.Any<GetPayslipDetailsCriteria>()).Returns(payslips);

            // Act
            var actual = await payslipService.SendPayslipAsync(viewModel);

            // Assert
            emailService.Received(2).SendEmail(Arg.Any<MailModel>());
        }

        #region Helpers

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private static ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId = 99;

            return service;
        }

        private static ICryptoService FakeCryptoService()
        {
            return Substitute.For<ICryptoService>();
        }

        private static IBaseMailService FakeMailService()
        {
            return Substitute.For<IBaseMailService>();
        }

        private static IViewRenderService FakeViewRenderService()
        {
            return Substitute.For<IViewRenderService>();
        }

        private static IPdfRenderService FakePdfRenderService()
        {
            return Substitute.For<IPdfRenderService>();
        }

        private static IPayslipRepository FakePayslipRepository()
        {
            return Substitute.For<IPayslipRepository>();
        }

        private PayslipService GetPayslipService()
        {
            baseService = FakeBaseService();
            sessionService = FakeSessionService();
            cryptoService = FakeCryptoService();
            payslipRepository = FakePayslipRepository();
            emailService = FakeMailService();
            viewRenderService = FakeViewRenderService();
            pdfRenderService = FakePdfRenderService();
            var service = new PayslipService(baseService, sessionService, cryptoService, emailService, viewRenderService, pdfRenderService, payslipRepository);

            return service;
        }

        #endregion Helpers
    }
}