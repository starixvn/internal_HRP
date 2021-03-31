namespace HRP.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Services;
    using HRP.Criteria.MonthlyPayroll;
    using HRP.Models;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.MonthlyPayroll.EmployeeMonthlyPayroll;
    using NSubstitute;
    using Xunit;

    public class MonthlyPayrollServiceTests
    {
        private ISessionService sessionService;
        private IBaseService baseService;
        private ICryptoService cryptoService;

        [Fact]
        public async Task GetFilterInfoAsync_WhenCalled_ReturnAsExpectedFilterInfo()
        {
            // Arrange
            var filterInfo = new MonthlyPayrollFilterInfoModel
            {
                PayrollId = 1,
                EmployeeId = 1,
                FullName = "a",
                Nickname = "b",
                IdNo = "c",
                CostCenter = "d",
                StartDate = new DateTime(2010, 1, 1),
                DueDate = new DateTime(2010, 1, 1),
                CompanyId = 1,
                CompanyCode = "DAR",
                WorkingDays = 22,
                IsHost = true,
                TotalEmployees = 100
            };
            var service = GetMonthlyPayrollService();
            baseService.GetDataEntityAsync<MonthlyPayrollFilterInfoModel>(Arg.Any<GetFilterInfoCriteria>()).Returns(filterInfo);

            // Act
            var actual = await service.GetFilterInfoAsync(1, DateTime.Today);

            // Assert
            Assert.Equal(filterInfo, actual);
        }

        [Fact]
        public async Task GetHierarchyCompanyAsync_WhenCalled_ReturnAsExpectedHierarchy()
        {
            // Arrange
            var divisions = new List<MultiSelectViewModel>
            {
                new MultiSelectViewModel { Value = "1", Text = "DAR", Checked = true }
            };
            var departments = new List<MultiSelectViewModel>
            {
                new MultiSelectViewModel { Value = "2", Text = "DAR", Checked = true, ParentId = "1" }
            };
            var costCenters = new List<MultiSelectViewModel>
            {
                new MultiSelectViewModel { Value = "3", Text = "DAR", Checked = true, ParentId = "2" }
            };
            var tuple = (divisions, departments, costCenters);
            var service = GetMonthlyPayrollService();
            baseService.FetchMultipleAsync<MultiSelectViewModel, MultiSelectViewModel, MultiSelectViewModel>(Arg.Any<GetHierarchyCompanyCriteria>()).Returns(tuple);

            // Act
            var actual = await service.GetHierarchyCompanyAsync(1, "1,2");

            // Assert
            Assert.Single(actual);
            Assert.Equal(divisions.First().Value, actual.First().Value);
            Assert.Equal(divisions.First().Text, actual.First().Text);
            Assert.Equal(divisions.First().Checked, actual.First().Checked);

            Assert.Single(actual.First().Items);
            Assert.Equal(departments.First().Value, actual.First().Items.First().Value);
            Assert.Equal(departments.First().Text, actual.First().Items.First().Text);
            Assert.Equal(departments.First().Checked, actual.First().Items.First().Checked);

            Assert.Single(actual.First().Items.First().Items);
            Assert.Equal(costCenters.First().Value, actual.First().Items.First().Items.First().Value);
            Assert.Equal(costCenters.First().Text, actual.First().Items.First().Items.First().Text);
            Assert.Equal(costCenters.First().Checked, actual.First().Items.First().Items.First().Checked);
        }

        #region GetMonthlyPayrollAsync

        [Fact]
        public async Task GetMonthlyPayrollAsync_WhenCalled_ReturnAsExpectedViewModel()
        {
            // Arrange
            var plainText = "1234";
            var viewModel = new EmployeeMonthlyPayrollViewModel
            {
                CompanyId = 1
            };
            var tuple = (
                    new List<EmployeeMonthlyPayrollModel> { new EmployeeMonthlyPayrollModel { PayrollId = 1 } },
                    new List<EmployeeMonthlyPayrollBasicSalaryModel> { new EmployeeMonthlyPayrollBasicSalaryModel { PayrollId = 1 } },
                    new List<EmployeeMonthlyPayrollBasicSalaryModel> { new EmployeeMonthlyPayrollBasicSalaryModel { PayrollId = 1 } });
            var monthlyPayrollService = GetMonthlyPayrollService();
            cryptoService.Decrypt(Arg.Any<string>()).Returns(plainText);
            baseService.FetchMultipleAsync<EmployeeMonthlyPayrollModel,
                    EmployeeMonthlyPayrollBasicSalaryModel,
                    EmployeeMonthlyPayrollBasicSalaryModel>(Arg.Any<GetMonthlyPayrollCriteria>()).Returns(tuple);

            // Act
            var actual = await monthlyPayrollService.GetMonthlyPayrollAsync(viewModel);

            // Assert
            Assert.Single(actual);
            Assert.Equal(tuple.Item1.First().PayrollId, actual.First().PayrollId);
            Assert.Equal(tuple.Item1.First().EmployeeId, actual.First().EmployeeId);
            Assert.Equal(tuple.Item1.First().IDNo, actual.First().IDNo);
            Assert.Equal(tuple.Item1.First().Nickname, actual.First().Nickname);
            Assert.Equal(tuple.Item1.First().FullName, actual.First().FullName);
            Assert.Equal(tuple.Item1.First().CostCenter, actual.First().CostCenter);
            Assert.Equal(tuple.Item1.First().WorkingDays, actual.First().WorkingDays);
            Assert.Equal(tuple.Item1.First().Policy, actual.First().Policy);

            Assert.Equal(tuple.Item1.First().BasicSalary.EbankingCurrencyCode, actual.First().BasicSalary.EbankingCurrencyCode);
            Assert.Equal("1,234.00", actual.First().BasicSalary.EbankingAmount);
            Assert.Equal(tuple.Item1.First().BasicSalary.DirectBanking1CurrencyCode, actual.First().BasicSalary.DirectBanking1CurrencyCode);
            Assert.Equal("1,234.00", actual.First().BasicSalary.DirectBanking1Amount);
            Assert.Equal(tuple.Item1.First().BasicSalary.DirectBanking2CurrencyCode, actual.First().BasicSalary.DirectBanking2CurrencyCode);
            Assert.Equal("1,234.00", actual.First().BasicSalary.DirectBanking2Amount);
            Assert.Equal(tuple.Item1.First().BasicSalary.CashCurrencyCode, actual.First().BasicSalary.CashCurrencyCode);
            Assert.Equal("1,234.00", actual.First().BasicSalary.CashAmount);

            Assert.Equal(tuple.Item1.First().BasicSalary.DirectBanking1CurrencyCode, actual.First().BasicSalary.DirectBanking1CurrencyCode);
            Assert.Equal("1,234.00", actual.First().BasicSalary.DirectBanking1Amount);
            Assert.Equal(tuple.Item1.First().BasicSalary.DirectBanking2CurrencyCode, actual.First().BasicSalary.DirectBanking2CurrencyCode);
            Assert.Equal("1,234.00", actual.First().BasicSalary.DirectBanking2Amount);
            Assert.Equal(tuple.Item1.First().BasicSalary.CashCurrencyCode, actual.First().BasicSalary.CashCurrencyCode);
            Assert.Equal("1,234.00", actual.First().BasicSalary.CashAmount);
        }

        #endregion GetMonthlyPayrollAsync

        [Fact]
        public async Task GetEmployeeSuggestionByNameAsync_WhenCalled_ReturnExpectedEmployees()
        {
            // Arrange
            var expected = new List<EmployeeSuggestionViewModel>
            {
                new EmployeeSuggestionViewModel { EmployeeId = 1, IDNo = "DAR111", FullName = "Tim", Nickname = "tim.trinh" }
            };
            var monthlyPayrollService = GetMonthlyPayrollService();
            baseService.GetDataAsync<EmployeeSuggestionViewModel>(Arg.Any<GetEmployeeSuggestionByNameCriteria>()).Returns(expected);

            // Act
            var actual = await monthlyPayrollService.GetEmployeeSuggestionByNameAsync(1, "tim");

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetEmployeeSuggestionByIDNoAsync_WhenCalled_ReturnExpectedEmployees()
        {
            // Arrange
            var expected = new List<EmployeeSuggestionViewModel>
            {
                new EmployeeSuggestionViewModel { EmployeeId = 1, IDNo = "DAR111", FullName = "Tim", Nickname = "tim.trinh" }
            };
            var monthlyPayrollService = GetMonthlyPayrollService();
            baseService.GetDataAsync<EmployeeSuggestionViewModel>(Arg.Any<GetEmployeeSuggestionByIDNoCriteria>()).Returns(expected);

            // Act
            var actual = await monthlyPayrollService.GetEmployeeSuggestionByIDNoAsync(1, "DAR111");

            // Assert
            Assert.Equal(expected, actual);
        }

        #region Helper

        private static ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId = 1;

            return service;
        }

        private MonthlyPayrollService GetMonthlyPayrollService()
        {
            sessionService = FakeSessionService();
            baseService = Substitute.For<IBaseService>();
            cryptoService = Substitute.For<ICryptoService>();
            var service = new MonthlyPayrollService(sessionService, baseService, cryptoService);

            return service;
        }

        #endregion Helper
    }
}