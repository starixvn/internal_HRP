namespace HRP.Repositories.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
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
        public async Task GetPayslipDetailsAsync_WhenCalled_ReturnAsExpectedModel()
        {
            // Arrange
            var multiResultReader = FakeMultipleResultReader();
            var criteria = FakeCriteria();
            var payslips = new List<PayslipDetailModel>
            {
                new PayslipDetailModel { PayrollId = 1, FullName = "thang" }
            };
            var salaryAdditions = new List<MonthlySalaryAdditionModel>
            {
                new MonthlySalaryAdditionModel { PayrollId = 1 }
            };
            var salaryDeductions = new List<MonthlySalaryDeductionModel>
            {
                new MonthlySalaryDeductionModel { PayrollId = 1 }
            };
            var allowanceAdditions = new List<MonthlyAllowanceDetailAdjustmentModel>
            {
                new MonthlyAllowanceDetailAdjustmentModel { PayrollId = 1 }
            };
            var allowanceDeductions = new List<MonthlyAllowanceDeductionModel>
            {
                new MonthlyAllowanceDeductionModel { PayrollId = 1 }
            };
            multiResultReader.ReadAsync<PayslipDetailModel>().Returns(payslips);
            multiResultReader.ReadAsync<MonthlySalaryAdditionModel>().Returns(salaryAdditions);
            multiResultReader.ReadAsync<MonthlySalaryDeductionModel>().Returns(salaryDeductions);
            multiResultReader.ReadAsync<MonthlyAllowanceDetailAdjustmentModel>().Returns(allowanceAdditions);
            multiResultReader.ReadAsync<MonthlyAllowanceDeductionModel>().Returns(allowanceDeductions);
            objectDb.QueryMultipleAsync(Arg.Any<ICriteria>()).Returns(multiResultReader);
            var repository = new PayslipRepository();

            // Act
            var actual = await repository.GetPayslipDetailsAsync(criteria);

            // Assert
            Assert.Single(actual);
            Assert.Single(actual.First().SalaryAdditions);
            Assert.Single(actual.First().SalaryDeductions);
            Assert.Single(actual.First().AllowanceAdditions);
            Assert.Single(actual.First().AllowanceDeductions);
            Assert.Equal(payslips.First().PayrollId, actual.First().PayrollId);
            Assert.Equal(payslips.First().Month, actual.First().Month);
            Assert.Equal(payslips.First().FullName, actual.First().FullName);
            Assert.Equal(payslips.First().IDNo, actual.First().IDNo);
            Assert.Equal(payslips.First().Email, actual.First().Email);
            Assert.Equal(payslips.First().CompanyName, actual.First().CompanyName);
            Assert.Equal(payslips.First().SalaryCurrencyId, actual.First().SalaryCurrencyId);
            Assert.Equal(payslips.First().SalaryCurrencyCode, actual.First().SalaryCurrencyCode);
            Assert.Equal(payslips.First().SalaryAmount, actual.First().SalaryAmount);
            Assert.Equal(payslips.First().AllowanceCurrencyId, actual.First().AllowanceCurrencyId);
            Assert.Equal(payslips.First().AllowanceCurrencyCode, actual.First().AllowanceCurrencyCode);
            Assert.Equal(payslips.First().AllowanceAmount, actual.First().AllowanceAmount);
            Assert.Equal(payslips.First().SalaryDeductionAmount, actual.First().SalaryDeductionAmount);
            Assert.Equal(payslips.First().AllowanceDeductionAmount, actual.First().AllowanceDeductionAmount);
            Assert.Equal(payslips.First().GrossSalaryAmount, actual.First().GrossSalaryAmount);
            Assert.Equal(payslips.First().GrossAllowanceAmount, actual.First().GrossAllowanceAmount);
            Assert.Equal(payslips.First().NetSalaryAmount, actual.First().NetSalaryAmount);
            Assert.Equal(payslips.First().NetAllowanceAmount, actual.First().NetAllowanceAmount);
            Assert.Equal(salaryAdditions.First(), actual.First().SalaryAdditions.First());
            Assert.Equal(salaryDeductions.First(), actual.First().SalaryDeductions.First());
            Assert.Equal(allowanceAdditions.First(), actual.First().AllowanceAdditions.First());
            Assert.Equal(allowanceDeductions.First(), actual.First().AllowanceDeductions.First());
        }

        #region Helpers

        private static IMultiResultReader FakeMultipleResultReader()
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