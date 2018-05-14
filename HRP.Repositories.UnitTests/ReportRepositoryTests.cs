namespace HRP.Repositories.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Models.Currency;
    using HRP.Models.Employee;
    using HRP.Models.MonthlyPayroll;
    using Models.Report;
    using NSubstitute;
    using Xunit;

    public class ReportRepositoryTests
    {
        private readonly IObjectDb objectDb;

        public ReportRepositoryTests()
        {
            DbSettingProviderManager
                .StartTestingSession()
                .Run();

            objectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.Mock((key, time) => objectDb);
        }

        #region Unit Tests

        [Fact]
        public async Task GetDataForBankInReportAsync_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var multiResultReader = FakeMultiResultReader();
            var criteria = FakeCriteria();

            multiResultReader.ReadAsync<BankIn>().Returns(new List<BankIn>());
            objectDb.QueryMultipleAsync(Arg.Any<ICriteria>()).Returns(multiResultReader);

            var repository = new ReportRepository();

            // Act
            var actual = await repository.GetDataForBankInReportAsync(criteria);

            // Assert
            Assert.IsType<BankInReport>(actual);
        }

        [Fact]
        public async Task GetDataForMonthlyPayrollReportAsync_WhenCalled_ReturnMonthlyPayrollReport()
        {
            // Arrange
            var employees = new List<EmployeeBasicInfo> { new EmployeeBasicInfo { CostCenterCode = "CC", SectionCode = "SC" } };
            var monthlyBasicSalary = new List<MonthlyBasicSalaryModel>();
            var monthlySalary = new List<MonthlySalaryModel>();
            var monthlyBasicAllowance = new List<MonthlyPayrollBasicAllowanceModel>();
            var monthlyAllowance = new List<MonthlyAllowanceModel>();
            var currencyExchangeRate = new List<CurrencyExchangeRateModel>();

            var criteria = FakeCriteria();
            var multiResult = FakeMultiResultReader();
            objectDb.QueryMultipleAsync(Arg.Any<ICriteria>()).Returns(multiResult);
            multiResult.ReadAsync<EmployeeBasicInfo>().Returns(employees);
            multiResult.ReadAsync<MonthlyBasicSalaryModel>().Returns(monthlyBasicSalary);
            multiResult.ReadAsync<MonthlySalaryModel>().Returns(monthlySalary);
            multiResult.ReadAsync<MonthlyPayrollBasicAllowanceModel>().Returns(monthlyBasicAllowance);
            multiResult.ReadAsync<MonthlyAllowanceModel>().Returns(monthlyAllowance);
            multiResult.ReadAsync<CurrencyExchangeRateModel>().Returns(currencyExchangeRate);
            var reportRepository = new ReportRepository();

            // Act
            var actual = await reportRepository.GetDataForMonthlyPayrollReportAsync(criteria);

            // Assert
            Assert.IsType<MonthlyPayrollReport>(actual);
            Assert.Equal(employees, actual.Employees);
            Assert.Equal(monthlyBasicSalary, actual.MonthlyBasicSalary);
            Assert.Equal(monthlySalary, actual.MonthlySalary);
            Assert.Equal(monthlyBasicAllowance, actual.MonthlyBasicAllowance);
            Assert.Equal(monthlyAllowance, actual.MonthlyAllowance);
            Assert.Equal(currencyExchangeRate, actual.CurrencyExchangeRate);
        }

        #endregion Unit Tests

        #region Helpers

        private static IMultiResultReader FakeMultiResultReader()
        {
            return Substitute.For<IMultiResultReader>();
        }

        private static ICriteria FakeCriteria()
        {
            var criteria = Substitute.For<ICriteria>();
            criteria.GetSettingKey().Returns("HRP_Test");

            return criteria;
        }

        #endregion Helpers
    }
}