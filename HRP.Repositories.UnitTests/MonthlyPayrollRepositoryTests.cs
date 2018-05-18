namespace HRP.Repositories.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
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

        #region GetMonthlySalaryAsync

        [Fact]
        public async Task GetMonthlySalaryToEditAsync_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var multiResultReader = FakeMultipleResultReader();
            var criteria = FakeCriteia();
            var monthlyBasicSalary = new MonthlyBasicSalaryModel();
            var monthlySalary = new MonthlySalaryModel
            {
                EbankingAccountName = "ACB",
                DirectBanking1AccountName = "CCC",
                DirectBanking2AccountName = "HSBC"
            };
            var additions = new List<MonthlySalaryAdditionModel>();
            var deductions = new List<MonthlySalaryDeductionModel>();
            multiResultReader.ReadFirstOrDefaultAsync<MonthlyBasicSalaryModel>().Returns(monthlyBasicSalary);
            multiResultReader.ReadFirstOrDefaultAsync<MonthlySalaryModel>().Returns(monthlySalary);
            multiResultReader.ReadAsync<MonthlySalaryAdditionModel>().Returns(additions);
            multiResultReader.ReadAsync<MonthlySalaryDeductionModel>().Returns(deductions);
            objectDb.QueryMultipleAsync(Arg.Any<ICriteria>()).Returns(multiResultReader);
            var repository = new MonthlyPayrollRepository();

            // Act
            var actual = await repository.GetMonthlySalaryAsync(criteria);

            // Assert
            Assert.Equal(monthlyBasicSalary, actual.Item1);
            Assert.Equal(monthlySalary, actual.Item2);
            Assert.Equal(additions, actual.Item3);
            Assert.Equal(deductions, actual.Item4);
        }

        #endregion GetMonthlySalaryAsync

        [Fact]
        public async Task GetMonthlyAllowancesAsync_NotExistedMonthlySalary_ReturnDefaultViewModel()
        {
            // Arrange
            var additions = new List<MonthlyAllowanceAdditionModel>
            {
                new MonthlyAllowanceAdditionModel
                {
                    PayrollId = 1,
                    NoOfDays = 20,
                    Type = 1,
                    NetAllowance = string.Empty,
                    TotalAmount = string .Empty,
                    UnitAmount = string.Empty,
                    CurrencyId = 1
                }
            };
            var deductions = new List<MonthlyAllowanceDeductionModel>
            {
                new MonthlyAllowanceDeductionModel
                {
                    PayrollId = 1,
                    NoOfDays = 20,
                    Type = 1,
                    NetAllowance = string.Empty,
                    TotalAmount = string .Empty,
                    UnitAmount = string.Empty,
                    CurrencyId = 1
                }
            };

            var allowance = new MonthlyAllowanceModel();
            var basicAllowance = new MonthlyPayrollBasicAllowanceModel();

            var multiResultReader = FakeMultipleResultReader();
            multiResultReader.ReadFirstOrDefault<MonthlyAllowanceModel>().Returns(allowance);
            multiResultReader.ReadFirstOrDefault<MonthlyPayrollBasicAllowanceModel>().Returns(basicAllowance);

            multiResultReader.Read<MonthlyAllowanceAdditionModel>().Returns(additions);
            multiResultReader.Read<MonthlyAllowanceDeductionModel>().Returns(deductions);

            var criteria = FakeCriteia();
            objectDb.QueryMultipleAsync(Arg.Any<ICriteria>()).Returns(multiResultReader);
            var repository = new MonthlyPayrollRepository();

            // Act
            var actual = await repository.GetMonthlyAllowancesAsync(criteria);

            // Assert
            Assert.NotNull(actual.Item1);
            Assert.NotEmpty(actual.Item2);
            Assert.NotEmpty(actual.Item3);
            Assert.NotNull(actual.Item4);
        }

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