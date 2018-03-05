namespace HRP.Services.UnitTests
{
    using System.Threading.Tasks;
    using HRP.Core.Services;
    using HRP.Criteria.MonthlyPayroll;
    using HRP.Models;
    using HRP.Repositories;
    using NSubstitute;
    using Xunit;

    public class MonthlyPayrollServiceTests
    {
        private ISessionService sessionService;
        private IMonthlyPayrollRepository monthlyPayrollRepository;

        #region GetDataFilterForIndexPageAsync

        [Theory]
        [InlineData(1, 0)]
        [InlineData(0, 1)]
        [InlineData(0, 0)]
        public async Task GetDataFilterForIndexPageAsync_ParamsAreInvalid_ReturnDefaultViewModel(int userId, int companyId)
        {
            // Arrange
            var monthlyPayrollService = GetMonthlyPayrollService();
            sessionService.UserId = userId;

            // Act
            var actual = await monthlyPayrollService.GetDataFilterForIndexPageAsync(companyId, Arg.Any<string>());

            // Assert
            Assert.IsType<EmployeeMonthlyPayrollViewModel>(actual);
            Assert.Equal(0, actual.CompanyId);
            Assert.Null(actual.CostCenterIds);
            Assert.Null(actual.EmployeeId);
            Assert.Empty(actual.HierarchyCompanyJson);
            Assert.Empty(actual.Employees);
            Assert.Empty(actual.HierarchyCompanies);
        }

        [Fact]
        public async Task GetDataFilterForIndexPageAsync_ParamsAreValid_ReturnAsExpectedViewModel()
        {
            // Arrange
            var viewModel = new EmployeeMonthlyPayrollViewModel
            {
                CostCenterIds = "1,2"
            };
            var monthlyPayrollService = GetMonthlyPayrollService();
            monthlyPayrollRepository.GetDataFilterForIndexPageAsync(Arg.Any<GetDataFilterForIndexPageCriteria>()).Returns(viewModel);

            // Act
            var actual = await monthlyPayrollService.GetDataFilterForIndexPageAsync(1, Arg.Any<string>());

            // Assert
            Assert.Equal(viewModel, actual);
        }

        #endregion GetDataFilterForIndexPageAsync

        #region Helper

        private static ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId = 1;

            return service;
        }

        private static IMonthlyPayrollRepository FakeMonthlyPayrollRepository()
        {
            return Substitute.For<IMonthlyPayrollRepository>();
        }

        private MonthlyPayrollService GetMonthlyPayrollService()
        {
            sessionService = FakeSessionService();
            monthlyPayrollRepository = FakeMonthlyPayrollRepository();
            var service = new MonthlyPayrollService(sessionService, monthlyPayrollRepository);

            return service;
        }

        #endregion Helper
    }
}