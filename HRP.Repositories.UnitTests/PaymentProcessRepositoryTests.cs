namespace HRP.Repositories.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Models.Currency;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.PaymentProcess;
    using HRP.Models.ServiceModels;
    using NSubstitute;
    using Xunit;

    public class PaymentProcessRepositoryTests
    {
        private readonly IObjectDb objectDb;

        public PaymentProcessRepositoryTests()
        {
            DbSettingProviderManager
            .StartTestingSession()
            .Run();

            objectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.Mock((key, time) => objectDb);
        }

        #region GetUploadingFilesResultAsync

        [Fact]
        public async Task GetUploadingFilesResultAsync_WhenCalled_ReturnExpectedTuple()
        {
            // Arrange
            var paymentProcess = MockPaymentProcess();
            var criteria = FakeCriteria();
            var multiResult = FakeMultiResultReader();
            var validationResults = MockListValidationResult();
            objectDb.QueryMultipleAsync(Arg.Any<ICriteria>()).Returns(multiResult);
            multiResult.ReadFirstOrDefaultAsync<PaymentProcess>().Returns(paymentProcess);
            multiResult.ReadAsync<ValidationResult>().Returns(validationResults);
            multiResult.ReadAsync<ValidationResult>().Returns(validationResults);
            multiResult.ReadAsync<ValidationResult>().Returns(validationResults);
            var repository = new PaymentProcessRepository();

            // Act
            var actual = await repository.GetUploadingFilesResultAsync(criteria);

            // Assert
            Assert.IsType<(PaymentProcess, IEnumerable<ValidationResult>, IEnumerable<ValidationResult>, IEnumerable<ValidationResult>)>(actual);
            Assert.Equal(paymentProcess, actual.Item1);
            Assert.Equal(validationResults, actual.Item2);
            Assert.Equal(validationResults, actual.Item3);
            Assert.Equal(validationResults, actual.Item4);
        }

        #endregion GetUploadingFilesResultAsync

        #region GetDataForGeneratePayrollAsync

        [Fact]
        public async Task GetDataForGeneratePayrollAsync_WhenCalled_ReturnGeneratePayrollModelAsync()
        {
            // Arrange
            var expected = new GeneratePayrollModel
            {
                MonthlyDSTNetSalary = new List<MonthlyDSTNetSalary> { new MonthlyDSTNetSalary { EmployeeId = 11, Month = DateTime.Today } },
                MonthlyAttendance = new List<MonthlyAttendance> { new MonthlyAttendance { EmployeeId = 11, Month = DateTime.Today, ActualWorkingDay = 23 } }
            };
            var criteria = FakeCriteria();
            var multiResult = FakeMultiResultReader();
            objectDb.QueryMultipleAsync(Arg.Any<ICriteria>()).Returns(multiResult);
            multiResult.ReadAsync<MonthlyPayroll>().Returns(expected.MonthlyPayroll);
            multiResult.ReadAsync<MonthlyAttendance>().Returns(expected.MonthlyAttendance);
            multiResult.ReadAsync<MonthlyDSTNetSalary>().Returns(expected.MonthlyDSTNetSalary);
            multiResult.ReadAsync<MonthlyBasicSalaryModel>().Returns(expected.MonthlyBasicSalary);
            multiResult.ReadAsync<MonthlyPayrollBasicAllowanceModel>().Returns(expected.MonthlyBasicAllowance);
            multiResult.ReadAsync<CurrencyExchangeRateModel>().Returns(expected.CurrencyExchangeRate);
            var repository = new PaymentProcessRepository();

            // Act
            var actual = await repository.GetDataForGeneratePayrollAsync(criteria);

            // Assert
            Assert.IsType<GeneratePayrollModel>(actual);
            Assert.Equal(expected.MonthlyPayroll, actual.MonthlyPayroll);
            Assert.Equal(expected.MonthlyAttendance, actual.MonthlyAttendance);
            Assert.Equal(expected.MonthlyDSTNetSalary, actual.MonthlyDSTNetSalary);
            Assert.Equal(expected.MonthlyBasicSalary, actual.MonthlyBasicSalary);
            Assert.Equal(expected.MonthlyBasicAllowance, actual.MonthlyBasicAllowance);
            Assert.Equal(expected.CurrencyExchangeRate, actual.CurrencyExchangeRate);
        }

        #endregion GetDataForGeneratePayroll

        #region Initialization

        private static IMultiResultReader FakeMultiResultReader()
        {
            return Substitute.For<IMultiResultReader>();
        }

        private static ICriteria FakeCriteria()
        {
            var criteria = Substitute.For<ICriteria>();

            return criteria;
        }

        private PaymentProcess MockPaymentProcess()
        {
            return new PaymentProcess
            {
                CompanyId = 2,
                Month = DateTime.Now,
                AttendanceFileName = "attendanceFile.xlsx",
                AttendanceTotalRow = 10,
                DSTNetSalaryFileName = "salaryFile.xls",
                DSTNetSalaryTotalRow = 12
            };
        }

        private IEnumerable<ValidationResult> MockListValidationResult()
        {
            return new List<ValidationResult>
            {
                new ValidationResult {RowNumber = 7, ResultType = 2, Message = "error"},
                new ValidationResult {RowNumber = 8, ResultType = 2, Message = "error2"}
            };
        }

        #endregion Initialization
    }
}