namespace HRP.Repositories.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Core.Model;
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

        [Fact]
        public async Task GetValidationFilesResultAsync_PaymentProcessIsNull_ReturnEmptyValidationFilesResultViewModel()
        {
            // Arrange
            PaymentProcess paymentProcess = null;
            var criteria = FakeCriteria();
            var multiResult = FakeMultiResultReader();
            objectDb.QueryMultipleAsync(Arg.Any<ICriteria>()).Returns(multiResult);
            multiResult.ReadFirstOrDefaultAsync<PaymentProcess>().Returns(paymentProcess);
            var repository = new PaymentProcessRepository();

            // Act
            var actual = await repository.GetValidationFilesResultAsync(criteria);

            // Assert
            Assert.IsType<ValidationFilesResultViewModel>(actual);
            Assert.Null(actual.Attendance);
            Assert.Null(actual.DSTNetSalary);
        }

        [Fact]
        public async Task GetValidationFilesResultAsync_AttendanceAndDSTSalaryFileNameAreNull_ReturnExpectedValidationFilesResultViewModel()
        {
            // Arrange
            var paymentProcess = new PaymentProcess();
            var criteria = FakeCriteria();
            var multiResult = FakeMultiResultReader();
            objectDb.QueryMultipleAsync(Arg.Any<ICriteria>()).Returns(multiResult);
            multiResult.ReadFirstOrDefaultAsync<PaymentProcess>().Returns(paymentProcess);
            var repository = new PaymentProcessRepository();

            // Act
            var actual = await repository.GetValidationFilesResultAsync(criteria);

            // Assert
            Assert.IsType<ValidationFilesResultViewModel>(actual);
            Assert.Null(actual.Attendance);
            Assert.Null(actual.DSTNetSalary);
        }

        [Fact]
        public async Task GetValidationFilesResultAsync_AttendanceAndDSTSalaryFileNameAreNotNull_ReturnExpectedValidationFilesResultViewModel()
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
            var actual = await repository.GetValidationFilesResultAsync(criteria);

            // Assert
            Assert.IsType<ValidationFilesResultViewModel>(actual);
            Assert.Equal(paymentProcess.AttendanceTotalRow, actual.Attendance.TotalRow);
            Assert.Equal(validationResults, actual.Attendance.ValidationErrors);
            Assert.Equal(paymentProcess.DSTNetSalaryTotalRow, actual.DSTNetSalary.TotalRow);
            Assert.Equal(validationResults, actual.DSTNetSalary.ValidationErrors);
        }

        [Fact]
        public async Task GetValidationFilesResultAsync_AttendanceFileNameIsNullAndDSTSalaryFileNameIsNotNull_ReturnExpectedValidationFilesResultViewModel()
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
            var actual = await repository.GetValidationFilesResultAsync(criteria);

            // Assert
            Assert.IsType<ValidationFilesResultViewModel>(actual);
            Assert.Null(actual.Attendance);
            Assert.Equal(paymentProcess.DSTNetSalaryTotalRow, actual.DSTNetSalary.TotalRow);
            Assert.Equal(validationResults, actual.DSTNetSalary.ValidationErrors);
        }

        [Fact]
        public async Task GetValidationFilesResultAsync_AttendanceFileNameIsNotNullAndDSTSalaryFileNameIsNull_ReturnExpectedValidationFilesResultViewModel()
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
            var actual = await repository.GetValidationFilesResultAsync(criteria);

            // Assert
            Assert.IsType<ValidationFilesResultViewModel>(actual);
            Assert.Null(actual.DSTNetSalary);
            Assert.Equal(paymentProcess.AttendanceTotalRow, actual.Attendance.TotalRow);
            Assert.Equal(validationResults, actual.Attendance.ValidationErrors);
        }

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