namespace HRP.Repositories.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Models;
    using HRP.Models.MonthlyPayroll;
    using NSubstitute;
    using Xunit;

    public class MonthlyPayrollRepositoryTests
    {
        private readonly IObjectDb objectDb;

        public MonthlyPayrollRepositoryTests()
        {
            DbSettingProviderManager
            .StartTestingSession()
            .Run();

            objectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.Mock((key, time) => objectDb);
        }

        [Fact]
        public async Task GetDataFilterForIndexPageAsync_WhenCalled_ReturnAsExpectedModel()
        {
            // Arrange
            var multiResultReader = FakeMultipleResultReader();
            var criteria = FakeCriteia();
            var viewModel = new EmployeeMonthlyPayrollViewModel();
            var multiSelects = new List<MultiSelectViewModel>
            {
                new MultiSelectViewModel { Value = "1", Text = "DAR", Checked = true }
            };
            multiResultReader.ReadFirstAsync<EmployeeMonthlyPayrollViewModel>().Returns(viewModel);
            multiResultReader.ReadAsync<MultiSelectViewModel>().Returns(multiSelects);
            objectDb.QueryMultiple(Arg.Any<ICriteria>()).Returns(multiResultReader);
            var repository = new MonthlyPayrollRepository();

            // Act
            var actual = await repository.GetDataFilterForIndexPageAsync(criteria);

            // Assert
            Assert.IsType<EmployeeMonthlyPayrollViewModel>(actual);
            Assert.Equal(viewModel.Month, actual.Month);
            Assert.Equal(viewModel.CostCenterIds, actual.CostCenterIds);
            Assert.Equal(viewModel.EmployeeId, actual.EmployeeId);
            Assert.Equal(viewModel.CompanyId, actual.CompanyId);
            Assert.Equal(viewModel.CompanyCode, actual.CompanyCode);
            Assert.Equal(viewModel.Employees, actual.Employees);
            Assert.Equal(viewModel.WorkingDays, actual.WorkingDays);
            Assert.Single(actual.HierarchyCompanies);
            Assert.Equal(multiSelects.First().Value, actual.HierarchyCompanies.First().Value);
            Assert.Equal(multiSelects.First().Text, actual.HierarchyCompanies.First().Text);
            Assert.Equal(multiSelects.First().Checked, actual.HierarchyCompanies.First().Checked);
            Assert.Equal(multiSelects.First().ParentId, actual.HierarchyCompanies.First().ParentId);
            Assert.Empty(actual.HierarchyCompanies.First().Items);
        }

        #region GetMonthlySalaryToUpdateAsync

        [Fact]
        public async Task GetMonthlySalaryToUpdateAsync_NotExistedMonthlySalary_ReturnDefaultViewModel()
        {
            // Arrange
            var multiResultReader = FakeMultipleResultReader();
            var criteria = FakeCriteia();
            UpdateMonthlySalaryViewModel expected = null;
            multiResultReader.ReadFirstOrDefaultAsync<UpdateMonthlySalaryViewModel>().Returns(expected);
            objectDb.QueryMultiple(Arg.Any<ICriteria>()).Returns(multiResultReader);
            var repository = new MonthlyPayrollRepository();

            // Act
            var actual = await repository.GetMonthlySalaryToUpdateAsync(criteria);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetMonthlySalaryToUpdateAsync_ExistedMonthlySalary_ReturnAsExpectedViewModel()
        {
            // Arrange
            var multiResultReader = FakeMultipleResultReader();
            var criteria = FakeCriteia();
            var viewModel = new UpdateMonthlySalaryViewModel();
            var monthlyBasicSalary = new MonthlyBasicSalaryViewModel();
            var monthlySalary = new MonthlySalaryViewModel();
            var additions = new List<MonthlySalaryAdditionViewModel>();
            var deductions = new List<MonthlySalaryDeductionViewModel>();
            multiResultReader.ReadFirstOrDefaultAsync<UpdateMonthlySalaryViewModel>().Returns(viewModel);
            multiResultReader.ReadFirstAsync<MonthlyBasicSalaryViewModel>().Returns(monthlyBasicSalary);
            multiResultReader.ReadFirstAsync<MonthlySalaryViewModel>().Returns(monthlySalary);
            multiResultReader.ReadAsync<MonthlySalaryAdditionViewModel>().Returns(additions);
            multiResultReader.ReadAsync<MonthlySalaryDeductionViewModel>().Returns(deductions);
            objectDb.QueryMultiple(Arg.Any<ICriteria>()).Returns(multiResultReader);
            var repository = new MonthlyPayrollRepository();

            // Act
            var actual = await repository.GetMonthlySalaryToUpdateAsync(criteria);

            // Assert
            Assert.Equal(viewModel.PayrollId, actual.PayrollId);
            Assert.Equal(viewModel.Month, actual.Month);
            Assert.Equal(viewModel.CompanyId, actual.CompanyId);
            Assert.Equal(viewModel.CompanyCode, actual.CompanyCode);
            Assert.Equal(monthlyBasicSalary, actual.MonthlyBasicSalary);
            Assert.Equal(monthlySalary, actual.MonthlySalary);
            Assert.Equal(additions, actual.Additions);
            Assert.Equal(deductions, actual.Deductions);
        }

        #endregion GetMonthlySalaryToUpdateAsync

        #region Helpers

        private static IMultiResultReader FakeMultipleResultReader()
        {
            return Substitute.For<IMultiResultReader>();
        }

        private static ICriteria FakeCriteia()
        {
            var criteria = Substitute.For<ICriteria>();
            criteria.GetSettingKey().Returns("HRP_Test");

            return criteria;
        }

        #endregion Helpers
    }
}