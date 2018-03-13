namespace HRP.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Extensions;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.Employee;
    using HRP.Criteria.PaymentProcess;
    using HRP.Models.PaymentProcess;
    using HRP.Repositories.Interfaces;
    using HRP.Resources;
    using Microsoft.AspNetCore.Http;
    using NPOI.SS.UserModel;
    using NSubstitute;
    using Xunit;

    public class PaymentProcessServiceTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private IPaymentProcessRepository paymentProcessRepository;
        private IObjectStream fileStream;
        private ITemplateReader templateReader;

        [Fact]
        public async Task CopyFileToMemoryStreamAsync_WhenCalled_ReturnMemoryStream()
        {
            // Arrange
            var stream = Substitute.For<Stream>();
            var paymentProcessService = GetPaymentProcessService();
            fileStream.CreateInstance(Arg.Any<string>()).Returns(stream);

            // Act
            var actual = await paymentProcessService.CopyFileToMemoryStreamAsync("ahihi");

            // Assert
            Assert.IsType<MemoryStream>(actual);
        }

        [Fact]
        public async Task IsPayrollGeneratedAsync_BaseServiceGetBoolInstance_ReturnBoolInstance()
        {
            // Arrange
            var expected = true;
            var paymentProcess = MockPaymentProcess();
            var criteria = new CheckPayrollGeneratedCriteria();
            var paymentProcessService = GetPaymentProcessService();
            baseService.GetDataEntityAsync<bool>(Arg.Do<CheckPayrollGeneratedCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await paymentProcessService.IsPayrollGeneratedAsync(paymentProcess);

            // Assert
            Assert.Equal(expected, actual);
            Assert.Equal(paymentProcess.CompanyId, criteria.CompanyId);
            Assert.Equal(paymentProcess.Month, criteria.Month);
        }

        #region CheckValidExcelFile

        [Fact]
        public void CheckValidExcelFile_FileIsNull_ReturnExpectedCommandResult()
        {
            // Arrange
            var paymentProcessService = GetPaymentProcessService();

            // Act
            var actual = paymentProcessService.CheckValidExcelFile(null);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.True(actual.IsSuccessful);
        }

        [Fact]
        public void CheckValidExcelFile_FileSizeLargerThan5Mb_ReturnErrorCommandResult()
        {
            // Arrange
            var excelFile = MockExcelFile(1024 * 1024 * 6);
            var paymentProcessService = GetPaymentProcessService();

            // Act
            var actual = paymentProcessService.CheckValidExcelFile(excelFile);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.False(actual.IsSuccessful);
            Assert.Equal(Message.FileTooLarge, actual.Message);
        }

        [Fact]
        public void CheckValidExcelFile_ParamIsNotAnExcelFile_ReturnErrorCommandResult()
        {
            // Arrange
            var file = MockTxtFile();
            var paymentProcessService = GetPaymentProcessService();

            // Act
            var actual = paymentProcessService.CheckValidExcelFile(file);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.False(actual.IsSuccessful);
            Assert.Equal(Message.InvalidFileType, actual.Message);
        }

        [Fact]
        public void CheckValidExcelFile_ParamIsExcelFile_ReturnSuccessfulCommandResult()
        {
            // Arrange
            var file = MockExcelFile();
            var paymentProcessService = GetPaymentProcessService();

            // Act
            var actual = paymentProcessService.CheckValidExcelFile(file);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.True(actual.IsSuccessful);
        }

        #endregion CheckValidExcelFile

        #region UploadAttendanceFileAsync

        [Fact]
        public async Task UploadAttendanceFileAsync_AttendanceFileIsNull_ReturnSuccessfulCommandResult()
        {
            // Arrange
            var paymentProcessService = GetPaymentProcessService();

            // Act
            var actual = await paymentProcessService.UploadAttendanceFileAsync(new UploadAttendanceAndSalaryViewModel { });

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.True(actual.IsSuccessful);
        }

        [Fact]
        public async Task UploadAttendanceFileAsync_ReadAttendanceFileFailure_ReturnErrorCommandResult()
        {
            // Arrange
            var viewModel = MockUploadAttendanceAndSalaryViewModel();
            var employeeCodeList = MockListString();
            var mockReadResult = MockFailureResultAttendanceUploading();
            var paymentProcessService = GetPaymentProcessService();
            baseService.GetDataAsync<string>(Arg.Is<GetAllEmployeeCodeByCompanyIdCriteria>(x => x.CompanyId == viewModel.CompanyId)).Returns(employeeCodeList);
            var stream = Substitute.For<Stream>();
            var sheet = Substitute.For<ISheet>();
            templateReader.CreateSheet(Arg.Any<Stream>()).Returns(sheet);
            templateReader.ReadAttendance(sheet, employeeCodeList, viewModel.CompanyCode).Returns(mockReadResult);

            // Act
            var actual = await paymentProcessService.UploadAttendanceFileAsync(viewModel);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.False(actual.IsSuccessful);
            Assert.Equal(actual.Message, mockReadResult.Message);
        }

        [Fact]
        public async Task UploadAttendanceFileAsync_ReadAttendanceFileSuccess_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var viewModel = MockUploadAttendanceAndSalaryViewModel();
            var employeeCodeList = MockListString();
            var mockReadResult = MockSuccessResultAttendanceUploading();
            var validationResult = MockListValidationResult();
            var criteria = new CreateAttendanceUploadingCriteria();

            var paymentProcessService = GetPaymentProcessService();
            baseService.GetDataAsync<string>(Arg.Is<GetAllEmployeeCodeByCompanyIdCriteria>(x => x.CompanyId == viewModel.CompanyId)).Returns(employeeCodeList);
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateAttendanceUploadingCriteria>(x => criteria = x)).Returns(expected);
            var stream = Substitute.For<Stream>();
            var sheet = Substitute.For<ISheet>();
            templateReader.CreateSheet(Arg.Any<Stream>()).Returns(sheet);
            templateReader.ReadAttendance(sheet, employeeCodeList, viewModel.CompanyCode).Returns(mockReadResult);
            templateReader.ValidationResults.Returns(validationResult);

            // Act
            var actual = await paymentProcessService.UploadAttendanceFileAsync(viewModel);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.True(actual.IsSuccessful);
            Assert.Equal(actual, expected);
            Assert.Equal(viewModel.CompanyId, criteria.CompanyId);
            Assert.Equal(viewModel.SelectedTime, criteria.Month);
            Assert.Equal(viewModel.AttendanceFile.FileName, criteria.FileName);
            Assert.Equal(mockReadResult.Value.ToXML().ToString(), criteria.UploadingXMLDoc);
            Assert.Equal(validationResult.ToXML().ToString(), criteria.ResultXMLDoc);
            Assert.Equal(sessionService.UserId, criteria.CreatedBy);
        }

        #endregion UploadAttendanceFileAsync

        #region GetValidationFilesResultAsync

        [Fact]
        public async Task GetValidationFilesResultAsync_CriteriaIsInvalid_ReturnEmptyValidationFilesResultViewModel()
        {
            // Arrange
            var companyId = 0;
            var month = DateTime.Now;
            var paymentProcessService = GetPaymentProcessService();

            // Act
            var actual = await paymentProcessService.GetValidationFilesResultAsync(companyId, month);

            // Assert
            Assert.IsType<ValidationFilesResultViewModel>(actual);
            Assert.Null(actual.Attendance);
            Assert.Null(actual.DSTNetSalary);
        }

        [Fact]
        public async Task GetValidationFilesResultAsync_CriteriaIsValid_ReturnExpectedValidationFilesResultViewModel()
        {
            // Arrange
            var expected = new ValidationFilesResultViewModel
            {
                Attendance = new FileResultViewModel { TotalRow = 2 },
                DSTNetSalary = new FileResultViewModel { TotalRow = 4 }
            };
            var companyId = 2;
            var month = DateTime.Now;
            var paymentProcessService = GetPaymentProcessService();
            paymentProcessRepository.GetValidationFilesResultAsync(Arg.Is<GetValidationFilesResultCriteria>(x => x.CompanyId == companyId && x.Month == month))
                .Returns(expected);

            // Act
            var actual = await paymentProcessService.GetValidationFilesResultAsync(companyId, month);

            // Assert
            Assert.IsType<ValidationFilesResultViewModel>(actual);
            Assert.Equal(expected, actual);
        }

        #endregion GetValidationFilesResultAsync

        #region Initialization

        private IPaymentProcessService GetPaymentProcessService()
        {
            baseService = FakeBaseService();
            sessionService = FakeSessionService();
            fileStream = FakeFileStream();
            templateReader = FakeTemplateReader();
            paymentProcessRepository = FakePaymentProcessRepository();
            var service = new PaymentProcessService(baseService, sessionService, paymentProcessRepository, fileStream, templateReader);

            return service;
        }

        private IBaseService FakeBaseService()
        {
            return Substitute.For<IBaseService>();
        }

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId.Returns(99);

            return service;
        }

        private IPaymentProcessRepository FakePaymentProcessRepository()
        {
            return Substitute.For<IPaymentProcessRepository>();
        }

        private IObjectStream FakeFileStream()
        {
            return Substitute.For<IObjectStream>();
        }

        private ITemplateReader FakeTemplateReader()
        {
            return Substitute.For<ITemplateReader>();
        }

        private PaymentProcess MockPaymentProcess()
        {
            return new PaymentProcess
            {
                CompanyId = 2,
                Month = DateTime.Now,
                AttendanceFileName = "attendanceFile.xlsx",
                DSTNetSalaryFileName = "salaryFile.xls"
            };
        }

        private CommandResult MockSuccessfulCommandResult()
        {
            return new CommandResult
            {
                Code = 0,
                Message = "successful"
            };
        }

        private IFormFile MockExcelFile(long fileSize = (1024 * 1024))
        {
            var file = Substitute.For<IFormFile>();
            var fileName = "template.xlsx";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write("ahihi");
            writer.Flush();
            ms.Position = 0;
            file.OpenReadStream().Returns(ms);
            file.FileName.Returns(fileName);
            file.Length.Returns(fileSize);
            file.ContentType.Returns(MimeType.XLSX);

            return file;
        }

        private static IFormFile MockTxtFile()
        {
            var file = Substitute.For<IFormFile>();
            var fileName = "test.txt";
            file.FileName.Returns(fileName);
            file.Length.Returns(1024);

            return file;
        }

        private IEnumerable<string> MockListString()
        {
            return new List<string>()
            {
                "ahihi",
                "dbbc"
            };
        }

        private UploadAttendanceAndSalaryViewModel MockUploadAttendanceAndSalaryViewModel()
        {
            return new UploadAttendanceAndSalaryViewModel
            {
                AttendanceFile = MockExcelFile(),
                CompanyCode = "Nex",
                CompanyId = 2,
                SelectedTime = DateTime.Now
            };
        }

        private Success<IEnumerable<AttendanceUploading>> MockSuccessResultAttendanceUploading()
        {
            var list = new List<AttendanceUploading>()
            {
                new AttendanceUploading {
                    RowNumber = 20,
                    EmployeeCode = "111",
                    ActualWorkingDay = "20",
                    LeaveTaken = "3",
                    UnpaidLeave =  "1",
                    LateInMinute = "2",
                    EarlyOutMinute =  "0"
                }
            };

            return new Success<IEnumerable<AttendanceUploading>>(list);
        }

        private Failure<IEnumerable<AttendanceUploading>> MockFailureResultAttendanceUploading()
        {
            return new Failure<IEnumerable<AttendanceUploading>>(Message.IncorrectTemplate);
        }

        private IEnumerable<ValidationResult> MockListValidationResult()
        {
            return new List<ValidationResult>()
            {
                new ValidationResult {RowNumber = 7, ResultType = 2, Message = "error"},
                new ValidationResult {RowNumber = 8, ResultType = 2, Message = "error2"}
            };
        }

        #endregion Initialization
    }
}