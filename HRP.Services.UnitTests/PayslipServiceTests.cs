namespace HRP.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.Payslip;
    using HRP.Models;
    using HRP.Models.Payslip;
    using NSubstitute;
    using Xunit;

    public class PayslipServiceTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;

        [Fact]
        public async Task GetCompanyAsync_WhenCalled_ReturnsAsExpectedCompanies()
        {
            // Arrange
            var companies = new List<Company>
            {
                new Company { CompanyId = 1, Code = "DAR" }
            };
            var payslipService = GetPayslipService();
            baseService.GetDataAsync<Company>(Arg.Is<GetCompanyByFunctionKeyCriteria>(x => x.UserId == sessionService.UserId && x.FunctionKey == "Payslip")).Returns(companies);

            // Act
            var actual = await payslipService.GetCompanyAsync();

            // Assert
            Assert.Equal(actual, companies);
        }

        [Fact]
        public async Task GetPayslipAsync_WhenCalled_ReturnsAsExpectedPayslips()
        {
            // Arrange
            var payslips = new List<PayslipModel>
            {
                new PayslipModel { PayrollId = 1, FullName = "thang", Nickname = "tim.trinh" }
            };
            var payslipService = GetPayslipService();
            baseService.GetDataAsync<PayslipModel>(Arg.Any<GetPayslipCriteria>()).Returns(payslips);

            // Act
            var actual = await payslipService.GetPayslipAsync(Arg.Any<DateTime>(), Arg.Any<int>(), Arg.Any<bool?>());

            // Assert
            Assert.Equal(actual, payslips);
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

        private PayslipService GetPayslipService()
        {
            baseService = FakeBaseService();
            sessionService = FakeSessionService();
            var service = new PayslipService(baseService, sessionService);

            return service;
        }

        #endregion Helpers
    }
}