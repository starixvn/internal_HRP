namespace HRP.Repositories.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.Payslip;
    using NSubstitute;
    using Xunit;

    public class PayslipRepositoryTests
    {
        private readonly IObjectDb objectDb;

        public PayslipRepositoryTests()
        {
            DbSettingProviderManager
            .StartTestingSession()
            .Run();

            objectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.Mock((key, time) => objectDb);
        }

        [Fact]
        public async Task GetPayslipDetailAsync_NotExistedPayroll_ReturnDefaultModel()
        {
            // Arrange
            var multiResultReader = FakeMultipleResultReader();
            var criteria = FakeCriteia();
            PayslipDetailModel expected = null;
            multiResultReader.ReadFirstOrDefaultAsync<PayslipDetailModel>().Returns(expected);
            objectDb.QueryMultipleAsync(Arg.Any<ICriteria>()).Returns(multiResultReader);
            var repository = new PayslipRepository();

            // Act
            var actual = await repository.GetPayslipDetailAsync(criteria);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetPayslipDetailAsync_ExistedPayroll_ReturnAsExpectedModel()
        {
            // Arrange
            var multiResultReader = FakeMultipleResultReader();
            var criteria = FakeCriteia();
            var model = new PayslipDetailModel();
            var salaryAdditions = new List<MonthlySalaryAdditionModel>();
            var salaryDeductions = new List<MonthlySalaryDeductionModel>();
            var allowanceAdditions = new List<MonthlyAllowanceAdditionModel>();
            var allowanceDeductions = new List<MonthlyAllowanceDeductionModel>();
            multiResultReader.ReadFirstOrDefaultAsync<PayslipDetailModel>().Returns(model);
            multiResultReader.ReadAsync<MonthlySalaryAdditionModel>().Returns(salaryAdditions);
            multiResultReader.ReadAsync<MonthlySalaryDeductionModel>().Returns(salaryDeductions);
            multiResultReader.ReadAsync<MonthlyAllowanceAdditionModel>().Returns(allowanceAdditions);
            multiResultReader.ReadAsync<MonthlyAllowanceDeductionModel>().Returns(allowanceDeductions);
            objectDb.QueryMultipleAsync(Arg.Any<ICriteria>()).Returns(multiResultReader);
            var repository = new PayslipRepository();

            // Act
            var actual = await repository.GetPayslipDetailAsync(criteria);

            // Assert
            Assert.Equal(model.PayrollId, actual.PayrollId);
            Assert.Equal(model.Month, actual.Month);
            Assert.Equal(model.FullName, actual.FullName);
            Assert.Equal(model.IDNo, actual.IDNo);
            Assert.Equal(model.CompanyName, actual.CompanyName);
            Assert.Equal(model.SalaryCurrencyId, actual.SalaryCurrencyId);
            Assert.Equal(model.SalaryCurrencyCode, actual.SalaryCurrencyCode);
            Assert.Equal(model.SalaryAmount, actual.SalaryAmount);
            Assert.Equal(model.AllowanceCurrencyId, actual.AllowanceCurrencyId);
            Assert.Equal(model.AllowanceCurrencyCode, actual.AllowanceCurrencyCode);
            Assert.Equal(model.AllowanceAmount, actual.AllowanceAmount);
            Assert.Equal(salaryAdditions, actual.SalaryAdditions);
            Assert.Equal(salaryDeductions, actual.SalaryDeductions);
            Assert.Equal(allowanceAdditions, actual.AllowanceAdditions);
            Assert.Equal(allowanceDeductions, actual.AllowanceDeductions);
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