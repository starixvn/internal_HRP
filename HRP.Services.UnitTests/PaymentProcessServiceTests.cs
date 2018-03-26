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
    using HRP.Criteria;
    using HRP.Criteria.Currency;
    using HRP.Criteria.Employee;
    using HRP.Criteria.PaymentProcess;
    using HRP.Models;
    using HRP.Models.Currency;
    using HRP.Models.PaymentProcess;
    using HRP.Repositories.Interfaces;
    using HRP.Resources;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NPOI.SS.UserModel;
    using NSubstitute;
    using Xunit;

    public class PaymentProcessServiceTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private ICryptoService cryptoService;
        private IPaymentProcessRepository paymentProcessRepository;
        private IObjectStream fileStream;
        private ITemplateReader templateReader;
        private IMonthlyPayrollService monthlyPayrollService;

        [Fact]
        public async Task GetCompanySelectListAsync_BaseServiceGetCompanies_ReturnCompanySelectList()
        {
            // Arrange
            var companies = new List<Company> { MockCompany() };
            var expected = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));
            var paymentProcessService = GetPaymentProcessService();
            baseService.GetDataAsync<Company>(Arg.Is<GetCompaniesForPaymentProcessCriteria>(x => x.UserId == sessionService.UserId)).Returns(companies);

            // Act
            var actual = await paymentProcessService.GetCompanySelectListAsync();

            // Assert
            Assert.IsType<SelectList>(actual);
            Assert.Equal(expected.DataValueField, actual.DataValueField);
            Assert.Equal(expected.DataTextField, actual.DataTextField);
            Assert.Equal(expected.Items, actual.Items);
        }

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

        [Fact]
        public async Task GetPaymentProcessAsync_WhenCalled_ReturnExpectedPaymenProcessInstance()
        {
            // Arrange
            var companyId = 21;
            var month = DateTime.Today;
            var expected = MockPaymentProcess();
            var paymentProcessService = GetPaymentProcessService();
            baseService.GetDataEntityAsync<PaymentProcess>(Arg.Is<GetPaymentProcessCriteria>(x => x.CompanyId == companyId
                && x.Month == month)).Returns(expected);

            // Act
            var actual = await paymentProcessService.GetPaymentProcessAsync(companyId, month);

            // Assert
            Assert.Equal(expected, actual);
        }

        #region ReadAttendanceFileAsync

        [Fact]
        public async Task ReadAttendanceFileAsync_AttendanceFileIsNull_ReturnSuccessResult()
        {
            // Arrange
            var paymentProcessService = GetPaymentProcessService();

            // Act
            var actual = await paymentProcessService.ReadAttendanceFileAsync(new UploadAttendanceAndSalaryViewModel { });

            // Assert
            var result = Assert.IsType<Success<AttendanceTemplate>>(actual);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task ReadAttendanceFileAsync_AttendanceFileSizeIsLargerThan5Mb_ReturnFailureResult()
        {
            // Arrange
            var commandResult = new CommandResult();
            var viewModel = new UploadAttendanceAndSalaryViewModel
            {
                AttendanceFile = MockExcelFile(1024 * 1024 * 6)
            };

            var paymentProcessService = GetPaymentProcessService();

            // Act
            var actual = await paymentProcessService.ReadAttendanceFileAsync(viewModel);

            // Assert
            var result = Assert.IsType<Failure<AttendanceTemplate>>(actual);
            Assert.Equal(Message.FileTooLarge, result.Message);
        }

        [Fact]
        public async Task ReadAttendanceFileAsync_AttendanceFileIsNotAnExcelFile_ReturnFailureResult()
        {
            // Arrange
            var commandResult = new CommandResult();
            var viewModel = new UploadAttendanceAndSalaryViewModel
            {
                AttendanceFile = MockTxtFile()
            };

            var paymentProcessService = GetPaymentProcessService();

            // Act
            var actual = await paymentProcessService.ReadAttendanceFileAsync(viewModel);

            // Assert
            var result = Assert.IsType<Failure<AttendanceTemplate>>(actual);
            Assert.Equal(Message.InvalidFileType, result.Message);
        }

        [Fact]
        public async Task ReadAttendanceFileAsync_AttendanceFileIsValidExcelFile_ReturnExpectedReadResult()
        {
            // Arrange
            var employeeCodeList = MockListString();
            var viewModel = MockUploadAttendanceAndSalaryViewModel();
            var readResult = MockFailureResultAttendanceTemplate();
            var paymentProcessService = GetPaymentProcessService();
            baseService.GetDataAsync<string>(Arg.Is<GetAllEmployeeCodeByCompanyIdCriteria>(x => x.CompanyId == viewModel.CompanyId)).Returns(employeeCodeList);
            var sheet = Substitute.For<ISheet>();
            templateReader.CreateSheet(Arg.Any<Stream>()).Returns(sheet);
            templateReader.ReadAttendance(Arg.Is<AttendanceTemplate>(x => x.Sheet == sheet && x.EmployeeCodeList == employeeCodeList && x.CompanyCode == viewModel.CompanyCode))
                .Returns(readResult);

            // Act
            var actual = await paymentProcessService.ReadAttendanceFileAsync(viewModel);

            // Assert
            var result = Assert.IsType<Failure<AttendanceTemplate>>(actual);
            Assert.Equal(readResult, result);
        }

        #endregion ReadAttendanceFileAsync

        #region ReadDSTNetSalaryFileAsync

        [Fact]
        public async Task ReadDSTNetSalaryFileAsync_DSTNetSalaryFileIsNull_ReturnSuccessResult()
        {
            // Arrange
            var paymentProcessService = GetPaymentProcessService();

            // Act
            var actual = await paymentProcessService.ReadDSTNetSalaryFileAsync(new UploadAttendanceAndSalaryViewModel { });

            // Assert
            var result = Assert.IsType<Success<DSTNetSalaryTemplate>>(actual);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task ReadDSTNetSalaryFileAsync_DSTNetSalaryFileIsNotAnExcelFile_ReturnFailureResult()
        {
            // Arrange
            var commandResult = new CommandResult();
            var viewModel = new UploadAttendanceAndSalaryViewModel
            {
                DSTNetSalaryFile = MockTxtFile()
            };

            var paymentProcessService = GetPaymentProcessService();

            // Act
            var actual = await paymentProcessService.ReadDSTNetSalaryFileAsync(viewModel);

            // Assert
            var result = Assert.IsType<Failure<DSTNetSalaryTemplate>>(actual);
            Assert.Equal(Message.InvalidFileType, result.Message);
        }

        [Fact]
        public async Task ReadDSTNetSalaryFileAsync_DSTNetSalaryFileIsValidExcelFile_ReturnExpectedReadResult()
        {
            // Arrange
            var employeeCodeList = MockListString();
            var currencies = MockCurrencies();
            var viewModel = MockUploadAttendanceAndSalaryViewModel();
            var readResult = MockFailureResultDSTNetSalaryTemplate();
            var paymentProcessService = GetPaymentProcessService();
            baseService.GetDataAsync<string>(Arg.Is<GetAllEmployeeCodeByCompanyIdCriteria>(x => x.CompanyId == viewModel.CompanyId)).Returns(employeeCodeList);
            baseService.GetDataAsync<Currency>(Arg.Is<GetAllCurrencyCriteria>(x => x.IsActive == true)).Returns(currencies);
            var sheet = Substitute.For<ISheet>();
            templateReader.CreateSheet(Arg.Any<Stream>()).Returns(sheet);
            templateReader.ReadDSTNetSalary(Arg.Is<DSTNetSalaryTemplate>(x => x.Sheet == sheet
                                        && x.EmployeeCodeList == employeeCodeList
                                        && x.CompanyCode == viewModel.CompanyCode
                                        && x.Currencies == currencies))
                                       .Returns(readResult);

            // Act
            var actual = await paymentProcessService.ReadDSTNetSalaryFileAsync(viewModel);

            // Assert
            var result = Assert.IsType<Failure<DSTNetSalaryTemplate>>(actual);
            Assert.Equal(readResult, result);
        }

        //[Fact]
        //public async Task UploadDSTNetSalaryFileAsync_ReadDSTNetSalaryFileSuccess_ReturnExpectedCommandResult()
        //{
        //    // Arrange
        //    var expected = MockSuccessfulCommandResult();
        //    var viewModel = MockUploadAttendanceAndSalaryViewModel();
        //    var employeeCodeList = MockListString();
        //    var currencies = MockCurrencies();
        //    var mockReadResult = MockSuccessResultDSTNetSalaryUploading();
        //    var validationResult = MockListValidationResult();
        //    var criteria = new CreateDSTNetSalaryUploadingCriteria();

        //    var paymentProcessService = GetPaymentProcessService();
        //    baseService.GetDataAsync<string>(Arg.Is<GetAllEmployeeCodeByCompanyIdCriteria>(x => x.CompanyId == viewModel.CompanyId)).Returns(employeeCodeList);
        //    baseService.GetDataAsync<Currency>(Arg.Is<GetAllCurrencyCriteria>(x => x.IsActive == true)).Returns(currencies);
        //    baseService.ExecuteNonQueryAsync(Arg.Do<CreateDSTNetSalaryUploadingCriteria>(x => criteria = x)).Returns(expected);

        //    var sheet = Substitute.For<ISheet>();
        //    templateReader.CreateSheet(Arg.Any<Stream>()).Returns(sheet);
        //    templateReader.ReadDSTNetSalary(Arg.Is<DSTNetSalaryTemplate>(x => x.Sheet == sheet
        //                                && x.EmployeeCodeList == employeeCodeList
        //                                && x.CompanyCode == viewModel.CompanyCode
        //                                && x.Currencies == currencies))
        //                               .Returns(mockReadResult);
        //    templateReader.DSTNetSalaryValidationResults.Returns(validationResult);

        //    // Act
        //    var actual = await paymentProcessService.UploadDSTNetSalaryFileAsync(viewModel);

        //    // Assert
        //    Assert.IsType<CommandResult>(actual);
        //    Assert.True(actual.IsSuccessful);
        //    Assert.Equal(actual, expected);
        //    Assert.Equal(viewModel.CompanyId, criteria.CompanyId);
        //    Assert.Equal(viewModel.SelectedTime, criteria.Month);
        //    Assert.Equal(viewModel.AttendanceFile.FileName, criteria.FileName);
        //    Assert.Equal(mockReadResult.Value.ToXML().ToString(), criteria.UploadingXMLDoc);
        //    Assert.Equal(validationResult.ToXML().ToString(), criteria.ResultXMLDoc);
        //    Assert.Equal(sessionService.UserId, criteria.CreatedBy); ;
        //}

        #endregion ReadDSTNetSalaryFileAsync

        #region GetUploadingFilesResultAsync

        [Fact]
        public async Task GetUploadingFilesResultAsync_CriteriaIsInvalid_ReturnEmptyUploadAttendanceAndSalaryViewModel()
        {
            // Arrange
            var expected = new UploadAttendanceAndSalaryViewModel();
            var companyId = 0;
            var month = DateTime.Now;
            var paymentProcessService = GetPaymentProcessService();

            // Act
            var actual = await paymentProcessService.GetUploadingFilesResultAsync(companyId, month);

            // Assert
            Assert.IsType<UploadAttendanceAndSalaryViewModel>(actual);
            Assert.Equal(expected.AttendanceFileName, actual.AttendanceFileName);
            Assert.Equal(expected.AttendanceResult, actual.AttendanceResult);
            Assert.Equal(expected.DSTNetSalaryFileName, actual.DSTNetSalaryFileName);
            Assert.Equal(expected.DSTNetSalaryResult, actual.DSTNetSalaryResult);
        }

        [Fact]
        public async Task GetUploadingFilesResultAsync_CriteriaIsValid_ReturnExpectedUploadAttendanceAndSalaryViewModel()
        {
            // Arrange
            var expected = new UploadAttendanceAndSalaryViewModel
            {
                AttendanceResult = new FileResultViewModel { TotalRow = 2 },
                DSTNetSalaryResult = new FileResultViewModel { TotalRow = 4 }
            };
            var companyId = 2;
            var month = DateTime.Now;
            var paymentProcessService = GetPaymentProcessService();
            paymentProcessRepository.GetUploadingFilesResultAsync(Arg.Is<GetUploadingFilesResultCriteria>(x => x.CompanyId == companyId && x.Month == month))
                .Returns(expected);

            // Act
            var actual = await paymentProcessService.GetUploadingFilesResultAsync(companyId, month);

            // Assert
            Assert.IsType<UploadAttendanceAndSalaryViewModel>(actual);
            Assert.Equal(expected, actual);
        }

        #endregion GetUploadingFilesResultAsync

        #region GenerateMonthlyPayroll

        [Fact]
        public async Task SubmitUploadingFilesAsync_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var companyId = 12;
            var month = DateTime.Today;
            var expected = MockSuccessfulCommandResult();
            var paymentProcessService = GetPaymentProcessService();
            baseService.ExecuteNonQueryAsync(Arg.Is<SubmitUploadingFilesCriteria>(x => x.CompanyId == companyId && x.Month == month))
                .Returns(expected);

            // Act
            var actual = await paymentProcessService.SubmitUploadingFilesAsync(companyId, month);

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion GenerateMonthlyPayroll

        #region Initialization

        private IPaymentProcessService GetPaymentProcessService()
        {
            baseService = FakeBaseService();
            sessionService = FakeSessionService();
            cryptoService = FakeCryptoService();
            fileStream = FakeFileStream();
            templateReader = FakeTemplateReader();
            paymentProcessRepository = FakePaymentProcessRepository();
            monthlyPayrollService = FakeMonthlyPayrollService();
            var service = new PaymentProcessService(baseService, sessionService, cryptoService, monthlyPayrollService, templateReader, paymentProcessRepository, fileStream);

            return service;
        }

        private IBaseService FakeBaseService()
        {
            return Substitute.For<IBaseService>();
        }

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId = 99;

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

        private IMonthlyPayrollService FakeMonthlyPayrollService()
        {
            return Substitute.For<IMonthlyPayrollService>();
        }

        private ICryptoService FakeCryptoService()
        {
            return Substitute.For<ICryptoService>();
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
                DSTNetSalaryFile = MockExcelFile(),
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
                    ActualWorkingDay = "20"
                }
            };

            return new Success<IEnumerable<AttendanceUploading>>(list);
        }

        private Failure<AttendanceTemplate> MockFailureResultAttendanceTemplate()
        {
            return new Failure<AttendanceTemplate>(Message.IncorrectTemplate);
        }

        private Failure<DSTNetSalaryTemplate> MockFailureResultDSTNetSalaryTemplate()
        {
            return new Failure<DSTNetSalaryTemplate>(Message.IncorrectTemplate);
        }

        private Success<IEnumerable<DSTNetSalaryUploading>> MockSuccessResultDSTNetSalaryUploading()
        {
            var list = new List<DSTNetSalaryUploading>()
            {
                new DSTNetSalaryUploading {
                    RowNumber = 8,
                    EmployeeCode = "111",
                    Currency = "VND",
                    NetSalaryReceived = "99999999"
                }
            };

            return new Success<IEnumerable<DSTNetSalaryUploading>>(list);
        }

        private IEnumerable<ValidationResult> MockListValidationResult()
        {
            return new List<ValidationResult>()
            {
                new ValidationResult {RowNumber = 7, ResultType = 2, Message = "error"},
                new ValidationResult {RowNumber = 8, ResultType = 2, Message = "error2"}
            };
        }

        private IEnumerable<Currency> MockCurrencies()
        {
            return new List<Currency>
            {
                new Currency {CurrencyCode = "USD"},
                new Currency {CurrencyCode = "VND"}
            };
        }

        private Company MockCompany()
        {
            return new Company
            {
                CompanyId = 11,
                Code = "company",
                Name = "descC",
                IsActive = true
            };
        }

        #endregion Initialization
    }
}