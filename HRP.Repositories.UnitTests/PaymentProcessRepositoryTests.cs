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
        public async Task GetUploadingFilesResultAsync_PaymentProcessIsNull_ReturnEmptyUploadAttendanceAndSalaryViewModel()
        {
            // Arrange
            PaymentProcess paymentProcess = null;
            var criteria = FakeCriteria();
            var multiResult = FakeMultiResultReader();
            objectDb.QueryMultipleAsync(Arg.Any<ICriteria>()).Returns(multiResult);
            multiResult.ReadFirstOrDefaultAsync<PaymentProcess>().Returns(paymentProcess);
            var repository = new PaymentProcessRepository();

            // Act
            var actual = await repository.GetUploadingFilesResultAsync(criteria);

            // Assert
            Assert.IsType<UploadAttendanceAndSalaryViewModel>(actual);
            Assert.Null(actual.CompanyCode);
            Assert.Null(actual.AttendanceFileName);
            Assert.Null(actual.AttendanceResult);
            Assert.Null(actual.DSTNetSalaryFileName);
            Assert.Null(actual.DSTNetSalaryResult);
        }

        [Fact]
        public async Task GetUploadingFilesResultAsync_AttendanceAndDSTSalaryFileNameAreNull_ReturnExpectedUploadAttendanceAndSalaryViewModel()
        {
            // Arrange
            var paymentProcess = new PaymentProcess { CompanyCode = "DAR" };
            var criteria = FakeCriteria();
            var multiResult = FakeMultiResultReader();
            objectDb.QueryMultipleAsync(Arg.Any<ICriteria>()).Returns(multiResult);
            multiResult.ReadFirstOrDefaultAsync<PaymentProcess>().Returns(paymentProcess);
            var repository = new PaymentProcessRepository();

            // Act
            var actual = await repository.GetUploadingFilesResultAsync(criteria);

            // Assert
            Assert.IsType<UploadAttendanceAndSalaryViewModel>(actual);
            Assert.Equal(paymentProcess.CompanyCode, actual.CompanyCode);
            Assert.Null(actual.AttendanceFileName);
            Assert.Null(actual.DSTNetSalaryFileName);
        }

        [Fact]
        public async Task GetUploadingFilesResultAsync_AttendanceAndDSTSalaryFileNameAreNotNull_ReturnExpectedUploadAttendanceAndSalaryViewModel()
        {
            // Arrange
            var paymentProcess = MockPaymentProcess();
            var criteria = FakeCriteria();
            var multiResult = FakeMultiResultReader();
            var validationResults = MockListValidationResult();
            objectDb.QueryMultipleAsync(Arg.Any<ICriteria>()).Returns(multiResult);
            multiResult.ReadFirstOrDefaultAsync<PaymentProcess>().Returns(paymentProcess);
            multiResult.ReadAsync<ValidationResult>().Returns(validationResults);
            var repository = new PaymentProcessRepository();

            // Act
            var actual = await repository.GetUploadingFilesResultAsync(criteria);

            // Assert
            Assert.IsType<UploadAttendanceAndSalaryViewModel>(actual);
            Assert.Equal(paymentProcess.CompanyCode, actual.CompanyCode);
            Assert.Equal(paymentProcess.AttendanceFileName, actual.AttendanceFileName);
            Assert.Equal(paymentProcess.AttendanceTotalRow, actual.AttendanceResult.TotalRow);
            Assert.Equal(validationResults, actual.AttendanceResult.ValidationErrors);
            Assert.Equal(paymentProcess.DSTNetSalaryFileName, actual.DSTNetSalaryFileName);
            Assert.Equal(paymentProcess.DSTNetSalaryTotalRow, actual.DSTNetSalaryResult.TotalRow);
            Assert.Equal(validationResults, actual.AttendanceResult.ValidationErrors);
        }

        [Fact]
        public async Task GetUploadingFilesResultAsync_AttendanceFileNameIsNullAndDSTSalaryFileNameIsNotNull_ReturnExpectedUploadAttendanceAndSalaryViewModel()
        {
            // Arrange
            var paymentProcess = new PaymentProcess { DSTNetSalaryFileName = "dstsalary.xls", DSTNetSalaryTotalRow = 9 };
            var criteria = FakeCriteria();
            var multiResult = FakeMultiResultReader();
            var validationResults = MockListValidationResult();
            objectDb.QueryMultipleAsync(Arg.Any<ICriteria>()).Returns(multiResult);
            multiResult.ReadFirstOrDefaultAsync<PaymentProcess>().Returns(paymentProcess);
            multiResult.ReadAsync<ValidationResult>().Returns(validationResults);
            var repository = new PaymentProcessRepository();

            // Act
            var actual = await repository.GetUploadingFilesResultAsync(criteria);

            // Assert
            Assert.IsType<UploadAttendanceAndSalaryViewModel>(actual);
            Assert.Null(actual.AttendanceFileName);
            Assert.Null(actual.AttendanceResult);
            Assert.Equal(paymentProcess.DSTNetSalaryFileName, actual.DSTNetSalaryFileName);
            Assert.Equal(paymentProcess.DSTNetSalaryTotalRow, actual.DSTNetSalaryResult.TotalRow);
            Assert.Equal(validationResults, actual.DSTNetSalaryResult.ValidationErrors);
        }

        [Fact]
        public async Task GetUploadingFilesResultAsync_AttendanceFileNameIsNotNullAndDSTSalaryFileNameIsNull_ReturnExpectedUploadAttendanceAndSalaryViewModel()
        {
            // Arrange
            var paymentProcess = new PaymentProcess { AttendanceFileName = "attendance.xlsx", AttendanceTotalRow = 7 };
            var criteria = FakeCriteria();
            var multiResult = FakeMultiResultReader();
            var validationResults = MockListValidationResult();
            objectDb.QueryMultipleAsync(Arg.Any<ICriteria>()).Returns(multiResult);
            multiResult.ReadFirstOrDefaultAsync<PaymentProcess>().Returns(paymentProcess);
            multiResult.ReadAsync<ValidationResult>().Returns(validationResults);
            var repository = new PaymentProcessRepository();

            // Act
            var actual = await repository.GetUploadingFilesResultAsync(criteria);

            // Assert
            Assert.IsType<UploadAttendanceAndSalaryViewModel>(actual);
            Assert.Null(actual.DSTNetSalaryResult);
            Assert.Null(actual.DSTNetSalaryFileName);
            Assert.Equal(paymentProcess.AttendanceFileName, actual.AttendanceFileName);
            Assert.Equal(paymentProcess.AttendanceTotalRow, actual.AttendanceResult.TotalRow);
            Assert.Equal(validationResults, actual.AttendanceResult.ValidationErrors);
        }

        #endregion GetUploadingFilesResultAsync

        #region GetDataForGeneratePayroll

        [Fact]
        public async Task GetDataForGeneratePayroll_WhenCalled_ReturnGeneratePayrollModelAsync()
        {
            // Arrange
            var expected = new GeneratePayrollModel();
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
            var actual = await repository.GetDataForGeneratePayroll(criteria);

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