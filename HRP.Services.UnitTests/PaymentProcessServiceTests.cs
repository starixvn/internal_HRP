namespace HRP.Services.UnitTests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.PaymentProcess;
    using HRP.Models.PaymentProcess;
    using HRP.Resources;
    using Microsoft.AspNetCore.Http;
    using NSubstitute;
    using Xunit;

    public class PaymentProcessServiceTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private IObjectStream fileStream;

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

        #endregion UploadAttendanceFileAsync

        #region Initialization

        private IPaymentProcessService GetPaymentProcessService()
        {
            baseService = FakeBaseService();
            sessionService = FakeSessionService();
            fileStream = FakeFileStream();
            var service = new PaymentProcessService(baseService, sessionService, null, fileStream);

            return service;
        }

        private IBaseService FakeBaseService()
        {
            return Substitute.For<IBaseService>();
        }

        private ISessionService FakeSessionService()
        {
            return Substitute.For<ISessionService>();
        }

        private IObjectStream FakeFileStream()
        {
            var stream = Substitute.For<IObjectStream>();

            return stream;
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
                Message = string.Empty
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

        #endregion Initialization
    }
}