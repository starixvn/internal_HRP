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
    using HRP.Models.Currency;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.PaymentProcess;
    using HRP.Models.ServiceModels;
    using HRP.Repositories;
    using HRP.Resources;
    using Microsoft.AspNetCore.Http;
    using NPOI.SS.UserModel;
    using NSubstitute;
    using Xunit;

    public class PaymentProcessServiceTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private ICryptoService cryptoService;
        private ICommonService commonService;
        private IPaymentProcessRepository paymentProcessRepository;
        private IObjectStream fileStream;
        private ITemplateReader templateReader;
        private IMonthlySalaryService monthlySalaryService;
        private IMonthlyAllowancesService monthlyAllowancesService;
        private IAdditionDeductionTemplateReader additionDeductionTemplateReader;

        [Fact]
        public async Task GetCurrentDuration_WhenCalled_ReturnExpectedPaymenProcessInstance()
        {
            // Arrange
            var expected = MockPaymentProcess();
            var paymentProcessService = GetPaymentProcessService();
            baseService.GetDataEntityAsync<PaymentProcess>(Arg.Any<GetCurrentPaymentProcessDurationCriteria>()).Returns(expected);

            // Act
            var actual = await paymentProcessService.GetCurrentDuration();

            // Assert
            Assert.Equal(expected, actual);
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
            var actual = await paymentProcessService.ReadAttendanceFileAsync(new UploadFilesViewModel { });

            // Assert
            var result = Assert.IsType<Success<AttendanceTemplate>>(actual);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task ReadAttendanceFileAsync_AttendanceFileSizeIsLargerThan5Mb_ReturnFailureResult()
        {
            // Arrange
            var viewModel = new UploadFilesViewModel
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
            var viewModel = new UploadFilesViewModel
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
            var actual = await paymentProcessService.ReadDSTNetSalaryFileAsync(new UploadFilesViewModel { });

            // Assert
            var result = Assert.IsType<Success<DSTNetSalaryTemplate>>(actual);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task ReadDSTNetSalaryFileAsync_DSTNetSalaryFileIsNotAnExcelFile_ReturnFailureResult()
        {
            // Arrange
            var viewModel = new UploadFilesViewModel
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
            commonService.GetCurrencyAsync().Returns(currencies);
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

        #endregion ReadDSTNetSalaryFileAsync

        #region GetUploadingFilesResultAsync

        [Fact]
        public async Task GetUploadingFilesResultAsync_CriteriaIsInvalid_ReturnEmptyUploadAttendanceAndSalaryViewModel()
        {
            // Arrange
            var expected = new UploadFilesViewModel();
            var companyId = 0;
            var month = DateTime.Now;
            var paymentProcessService = GetPaymentProcessService();

            // Act
            var actual = await paymentProcessService.GetUploadingFilesResultAsync(companyId, month);

            // Assert
            Assert.IsType<UploadFilesViewModel>(actual);
            Assert.Equal(expected.AttendanceFileName, actual.AttendanceFileName);
            Assert.Equal(expected.AttendanceResult, actual.AttendanceResult);
            Assert.Equal(expected.DSTNetSalaryFileName, actual.DSTNetSalaryFileName);
            Assert.Equal(expected.DSTNetSalaryResult, actual.DSTNetSalaryResult);
        }

        [Fact]
        public async Task GetUploadingFilesResultAsync_PaymentProcessIsNull_ReturnEmptyUploadAttendanceAndSalaryViewModel()
        {
            // Arrange
            PaymentProcess paymentProcess = null;
            IEnumerable<ValidationResult> attendanceErrors = null;
            IEnumerable<ValidationResult> dstNetSalaryErrors = null;
            IEnumerable<ValidationResult> additionDeductionErrors = null;
            var expected = new UploadFilesViewModel();
            var companyId = 2;
            var month = DateTime.Now;
            var paymentProcessService = GetPaymentProcessService();
            paymentProcessRepository.GetUploadingFilesResultAsync(Arg.Is<GetUploadingFilesResultCriteria>(x => x.CompanyId == companyId && x.Month == month))
                .Returns((paymentProcess, attendanceErrors, dstNetSalaryErrors, additionDeductionErrors));

            // Act
            var actual = await paymentProcessService.GetUploadingFilesResultAsync(companyId, month);

            // Assert
            Assert.IsType<UploadFilesViewModel>(actual);
            Assert.Equal(expected.AttendanceFileName, actual.AttendanceFileName);
            Assert.Equal(expected.AttendanceResult, actual.AttendanceResult);
            Assert.Equal(expected.DSTNetSalaryFileName, actual.DSTNetSalaryFileName);
            Assert.Equal(expected.DSTNetSalaryResult, actual.DSTNetSalaryResult);
        }

        [Fact]
        public async Task GetUploadingFilesResultAsync_AttendanceAndDSTSalaryFileNameAreNull_ReturnExpectedUploadAttendanceAndSalaryViewModel()
        {
            // Arrange
            var paymentProcess = new PaymentProcess
            {
                CompanyId = 2,
                Month = DateTime.Now,
                CompanyCode = "DAR",
                StartDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(1)
            };

            IEnumerable<ValidationResult> attendanceErrors = null;
            IEnumerable<ValidationResult> dstNetSalaryErrors = null;
            IEnumerable<ValidationResult> additionDeductionErrors = null;
            var companyId = 2;
            var month = DateTime.Now;
            var paymentProcessService = GetPaymentProcessService();
            paymentProcessRepository.GetUploadingFilesResultAsync(Arg.Is<GetUploadingFilesResultCriteria>(x => x.CompanyId == companyId && x.Month == month))
                .Returns((paymentProcess, attendanceErrors, dstNetSalaryErrors, additionDeductionErrors));

            // Act
            var actual = await paymentProcessService.GetUploadingFilesResultAsync(companyId, month);

            // Assert
            Assert.IsType<UploadFilesViewModel>(actual);
            Assert.Equal(paymentProcess.CompanyId, actual.CompanyId);
            Assert.Equal(paymentProcess.Month, actual.SelectedTime);
            Assert.Equal(paymentProcess.CompanyCode, actual.CompanyCode);
            Assert.Equal(paymentProcess.StartDate, actual.StartDate);
            Assert.Equal(paymentProcess.DueDate, actual.DueDate);
            Assert.Null(actual.AttendanceFileName);
            Assert.Null(actual.AttendanceResult);
            Assert.Null(actual.DSTNetSalaryFileName);
            Assert.Null(actual.DSTNetSalaryResult);
        }

        [Fact]
        public async Task GetUploadingFilesResultAsync_AttendanceAndDSTSalaryFileNameAreNotNull_ReturnExpectedUploadAttendanceAndSalaryViewModel()
        {
            // Arrange
            var paymentProcess = new PaymentProcess
            {
                CompanyId = 2,
                Month = DateTime.Now,
                CompanyCode = "DAR",
                StartDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(1),
                AttendanceFileName = "attendanceFile.xlsx",
                AttendanceTotalRow = 10,
                DSTNetSalaryFileName = "salaryFile.xls",
                DSTNetSalaryTotalRow = 12
            };
            var validationResults = MockListValidationResult();
            IEnumerable<ValidationResult> additionDeductionErrors = null;
            var companyId = 2;
            var month = DateTime.Now;
            var paymentProcessService = GetPaymentProcessService();
            paymentProcessRepository.GetUploadingFilesResultAsync(Arg.Is<GetUploadingFilesResultCriteria>(x => x.CompanyId == companyId && x.Month == month))
                .Returns((paymentProcess, validationResults, validationResults, additionDeductionErrors));

            // Act
            var actual = await paymentProcessService.GetUploadingFilesResultAsync(companyId, month);

            // Assert
            Assert.IsType<UploadFilesViewModel>(actual);
            Assert.Equal(paymentProcess.CompanyId, actual.CompanyId);
            Assert.Equal(paymentProcess.Month, actual.SelectedTime);
            Assert.Equal(paymentProcess.CompanyCode, actual.CompanyCode);
            Assert.Equal(paymentProcess.StartDate, actual.StartDate);
            Assert.Equal(paymentProcess.DueDate, actual.DueDate);
            Assert.Equal(paymentProcess.AttendanceFileName, actual.AttendanceFileName);
            Assert.Equal(paymentProcess.AttendanceTotalRow, actual.AttendanceResult.TotalRow);
            Assert.Equal(validationResults, actual.AttendanceResult.ValidationErrors);
            Assert.Equal(paymentProcess.DSTNetSalaryFileName, actual.DSTNetSalaryFileName);
            Assert.Equal(paymentProcess.DSTNetSalaryTotalRow, actual.DSTNetSalaryResult.TotalRow);
            Assert.Equal(validationResults, actual.DSTNetSalaryResult.ValidationErrors);
        }

        [Fact]
        public async Task GetUploadingFilesResultAsync_AttendanceFileNameIsNullAndDSTSalaryFileNameIsNotNull_ReturnExpectedUploadAttendanceAndSalaryViewModel()
        {
            // Arrange
            var paymentProcess = new PaymentProcess
            {
                CompanyId = 2,
                Month = DateTime.Now,
                CompanyCode = "DAR",
                StartDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(1),
                DSTNetSalaryFileName = "salaryFile.xls",
                DSTNetSalaryTotalRow = 12
            };
            var validationResults = MockListValidationResult();
            IEnumerable<ValidationResult> attendanceErrors = null;
            IEnumerable<ValidationResult> additionDeductionErrors = null;
            var companyId = 2;
            var month = DateTime.Now;
            var paymentProcessService = GetPaymentProcessService();
            paymentProcessRepository.GetUploadingFilesResultAsync(Arg.Is<GetUploadingFilesResultCriteria>(x => x.CompanyId == companyId && x.Month == month))
                .Returns((paymentProcess, attendanceErrors, validationResults, additionDeductionErrors));

            // Act
            var actual = await paymentProcessService.GetUploadingFilesResultAsync(companyId, month);

            // Assert
            Assert.IsType<UploadFilesViewModel>(actual);
            Assert.Equal(paymentProcess.CompanyId, actual.CompanyId);
            Assert.Equal(paymentProcess.Month, actual.SelectedTime);
            Assert.Equal(paymentProcess.CompanyCode, actual.CompanyCode);
            Assert.Equal(paymentProcess.StartDate, actual.StartDate);
            Assert.Equal(paymentProcess.DueDate, actual.DueDate);
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
            var paymentProcess = new PaymentProcess
            {
                CompanyId = 2,
                Month = DateTime.Now,
                CompanyCode = "DAR",
                StartDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(1),
                AttendanceFileName = "attendanceFile.xlsx",
                AttendanceTotalRow = 10,
            };
            var validationResults = MockListValidationResult();
            IEnumerable<ValidationResult> dstNetSalaryErrors = null;
            IEnumerable<ValidationResult> additionDeductionErrors = null;
            var companyId = 2;
            var month = DateTime.Now;
            var paymentProcessService = GetPaymentProcessService();
            paymentProcessRepository.GetUploadingFilesResultAsync(Arg.Is<GetUploadingFilesResultCriteria>(x => x.CompanyId == companyId && x.Month == month))
                .Returns((paymentProcess, validationResults, dstNetSalaryErrors, additionDeductionErrors));

            // Act
            var actual = await paymentProcessService.GetUploadingFilesResultAsync(companyId, month);

            // Assert
            Assert.IsType<UploadFilesViewModel>(actual);
            Assert.Equal(paymentProcess.CompanyId, actual.CompanyId);
            Assert.Equal(paymentProcess.Month, actual.SelectedTime);
            Assert.Equal(paymentProcess.CompanyCode, actual.CompanyCode);
            Assert.Equal(paymentProcess.StartDate, actual.StartDate);
            Assert.Equal(paymentProcess.DueDate, actual.DueDate);
            Assert.Equal(paymentProcess.AttendanceFileName, actual.AttendanceFileName);
            Assert.Equal(paymentProcess.AttendanceTotalRow, actual.AttendanceResult.TotalRow);
            Assert.Equal(validationResults, actual.AttendanceResult.ValidationErrors);
            Assert.Null(actual.DSTNetSalaryResult);
            Assert.Null(actual.DSTNetSalaryFileName);
        }

        #endregion GetUploadingFilesResultAsync

        #region UploadFilesAsync

        [Fact]
        public async Task UploadFilesAsync_TemplatesAreNotNull_ReturnExpectedCommandResult()
        {
            // Arrange
            var companyId = 1;
            var month = DateTime.Now;
            var attendanceTemplate = new AttendanceTemplate
            {
                FileName = "attendance.xls",
                Records = MockListAttendanceUploading(),
                ValidationResults = MockListValidationResult()
            };
            var dstNetSalaryTemplate = new DSTNetSalaryTemplate
            {
                FileName = "dst.xls",
                Records = MockListDSTNetSalaryUploading(),
                ValidationResults = MockListValidationResult()
            };
            var additionDeductionTemplate = new AdditionDeductionTemplate
            {
                FileName = "addition - deduction.xlsx"
            };

            var paymentProcessService = GetPaymentProcessService();
            baseService.ExecuteNonQueryAsync(Arg.Is<UploadFilesCommand>(x => x.CompanyId == companyId && x.Month == month
                && x.AttendanceFileName == attendanceTemplate.FileName
                && x.AttendanceUploadingXMLDoc == attendanceTemplate.Records.ToXML().ToString()
                && x.AttendanceResultXMLDoc == attendanceTemplate.ValidationResults.ToXML().ToString()
                && x.DSTNetSalaryFileName == dstNetSalaryTemplate.FileName
                && x.DSTNetSalaryUploadingXMLDoc == dstNetSalaryTemplate.Records.ToXML().ToString()
                && x.DSTNetSalaryResultXMLDoc == dstNetSalaryTemplate.ValidationResults.ToXML().ToString()
            )).Returns(new CommandResult());

            // Act
            var actual = await paymentProcessService.UploadFilesAsync(companyId, month, attendanceTemplate, dstNetSalaryTemplate, additionDeductionTemplate);

            // Assert
            var result = Assert.IsType<CommandResult>(actual);
            Assert.False(result.IsSuccessful);
        }

        #endregion UploadFilesAsync

        #region DeleteUploadedAttendance

        [Fact]
        public async Task DeleteUploadedAttendance_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var companyId = 2;
            var month = DateTime.Today;
            var expected = new CommandResult();
            var paymentProcessService = GetPaymentProcessService();
            baseService.ExecuteNonQueryAsync(Arg.Is<DeleteUploadedAttendanceCriteria>(x => x.CompanyId == companyId && x.Month == month)).Returns(expected);

            // Act
            var actual = await paymentProcessService.DeleteUploadedAttendance(companyId, month);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.False(actual.IsSuccessful);
        }

        #endregion DeleteUploadedAttendance

        #region DeleteUploadedDSTNetSalary

        [Fact]
        public async Task DeleteUploadedDSTNetSalary_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var companyId = 2;
            var month = DateTime.Today;
            var expected = new CommandResult();
            var paymentProcessService = GetPaymentProcessService();
            baseService.ExecuteNonQueryAsync(Arg.Is<DeleteUploadedDSTNetSalaryCriteria>(x => x.CompanyId == companyId && x.Month == month)).Returns(expected);

            // Act
            var actual = await paymentProcessService.DeleteUploadedDSTNetSalary(companyId, month);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.False(actual.IsSuccessful);
        }

        #endregion DeleteUploadedDSTNetSalary

        #region GenerateMonthlyPayroll

        [Fact]
        public async Task GenerateMonthlyPayrollAsync_MonthlyBasicSalaryIsNull_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var generatePayrollViewModel = new GeneratePayrollModel
            {
                MonthlyPayroll = new List<MonthlyPayroll> { new MonthlyPayroll { EmployeeId = 1, Month = DateTime.Today } },
                MonthlyBasicSalary = new List<MonthlyBasicSalaryModel>(),
                MonthlyBasicAllowance = new List<MonthlyPayrollBasicAllowanceModel>(),
                MonthlyAttendance = new List<MonthlyAttendance>(),
                MonthlyDSTNetSalary = new List<MonthlyDSTNetSalary>(),
                CurrencyExchangeRate = new List<CurrencyExchangeRateModel>()
            };

            var companyId = 3;
            var month = DateTime.Today;
            var employeeId = 1;
            var paymentProcessService = GetPaymentProcessService();
            paymentProcessRepository.GetDataForGeneratePayroll(Arg.Is<GetDataForGeneratePayrollCriteria>(x => x.CompanyId == companyId && x.Month == month && x.EmployeeId == employeeId))
                .Returns(generatePayrollViewModel);
            baseService.ExecuteNonQueryAsync(Arg.Is<GenerateMonthlyPayrollCriteria>(x => x.CompanyId == companyId && x.Month == month)).Returns(expected);

            // Act
            var actual = await paymentProcessService.GenerateMonthlyPayrollAsync(companyId, month, employeeId);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GenerateMonthlyPayrollAsync_MonthlyBasicSalaryHasValue_ReturnExpectedCommandResult()
        {
            // Arrange
            var companyId = 3;
            var month = DateTime.Today;
            var employeeId = 1;
            var expected = new CommandResult();
            var generatePayrollViewModel = new GeneratePayrollModel
            {
                MonthlyPayroll = new List<MonthlyPayroll> { new MonthlyPayroll { EmployeeId = employeeId, Month = month } },
                MonthlyBasicSalary = new List<MonthlyBasicSalaryModel> { new MonthlyBasicSalaryModel { EmployeeId = employeeId } },
                MonthlyBasicAllowance = new List<MonthlyPayrollBasicAllowanceModel> { new MonthlyPayrollBasicAllowanceModel { EmployeeId = employeeId } },
                MonthlyAttendance = new List<MonthlyAttendance>(),
                MonthlyDSTNetSalary = new List<MonthlyDSTNetSalary> { new MonthlyDSTNetSalary { EmployeeId = employeeId } },
                CurrencyExchangeRate = new List<CurrencyExchangeRateModel>()
            };
            var mockAllowance = Substitute.For<MonthlyAllowanceModel>();
            var allowanceAdditions = new List<MonthlyAllowanceAdditionModel> { new MonthlyAllowanceAdditionModel() };
            var allowanceDeductions = new List<MonthlyAllowanceDeductionModel> { new MonthlyAllowanceDeductionModel() };
            var paymentProcessService = GetPaymentProcessService();
            paymentProcessRepository.GetDataForGeneratePayroll(Arg.Is<GetDataForGeneratePayrollCriteria>(x => x.CompanyId == companyId && x.Month == month && x.EmployeeId == employeeId))
                .Returns(generatePayrollViewModel);
            baseService.ExecuteNonQueryAsync(Arg.Is<GenerateMonthlyPayrollCriteria>(x => x.CompanyId == companyId && x.Month == month)).Returns(expected);
            monthlyAllowancesService.InitializeMonthlyAllowance(Arg.Any<MonthlyPayrollBasicAllowanceModel>(),
                Arg.Any<IEnumerable<MonthlyAllowanceAdditionModel>>(),
                Arg.Any<IEnumerable<MonthlyAllowanceDeductionModel>>(),
                Arg.Any<IEnumerable<CurrencyExchangeRateModel>>(),
                Arg.Any<MonthlyDSTNetSalary>()).Returns(mockAllowance);
            monthlyAllowancesService.InitializeAllowanceAdditions(Arg.Any<int>(), Arg.Any<decimal>(),
                Arg.Any<MonthlyPayrollBasicAllowanceModel>(), Arg.Any<IEnumerable<CurrencyExchangeRateModel>>()).Returns(allowanceAdditions);
            monthlyAllowancesService.InitializeAllowanceDeductions(Arg.Any<int>(), Arg.Any<decimal>(),
                Arg.Any<MonthlyPayrollBasicAllowanceModel>(), Arg.Any<IEnumerable<CurrencyExchangeRateModel>>()).Returns(allowanceDeductions);

            // Act
            var actual = await paymentProcessService.GenerateMonthlyPayrollAsync(companyId, month, employeeId);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
        }

        #endregion GenerateMonthlyPayroll

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

        #region Initialization

        private IPaymentProcessService GetPaymentProcessService()
        {
            baseService = Substitute.For<IBaseService>();
            sessionService = FakeSessionService();
            cryptoService = Substitute.For<ICryptoService>();
            commonService = Substitute.For<ICommonService>();
            fileStream = Substitute.For<IObjectStream>();
            templateReader = Substitute.For<ITemplateReader>();
            paymentProcessRepository = Substitute.For<IPaymentProcessRepository>();
            monthlySalaryService = Substitute.For<IMonthlySalaryService>();
            monthlyAllowancesService = Substitute.For<IMonthlyAllowancesService>();
            additionDeductionTemplateReader = Substitute.For<IAdditionDeductionTemplateReader>();
            var service = new PaymentProcessService(
                baseService,
                sessionService,
                cryptoService,
                commonService,
                monthlySalaryService,
                monthlyAllowancesService,
                templateReader,
                paymentProcessRepository,
                fileStream,
                additionDeductionTemplateReader);

            return service;
        }

        private static ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId = 99;

            return service;
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

        private static IFormFile MockExcelFile(long fileSize = (1024 * 1024))
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

        private static IEnumerable<string> MockListString()
        {
            return new List<string>
            {
                "ahihi",
                "dbbc"
            };
        }

        private UploadFilesViewModel MockUploadAttendanceAndSalaryViewModel()
        {
            return new UploadFilesViewModel
            {
                AttendanceFile = MockExcelFile(),
                DSTNetSalaryFile = MockExcelFile(),
                CompanyCode = "Nex",
                CompanyId = 2,
                SelectedTime = DateTime.Now,
                CommonError = string.Empty
            };
        }

        private IEnumerable<AttendanceUploading> MockListAttendanceUploading()
        {
            return new List<AttendanceUploading>
            {
                new AttendanceUploading {
                    RowNumber = 20,
                    EmployeeCode = "111",
                    ActualWorkingDay = "20"
                }
            };
        }

        private IEnumerable<DSTNetSalaryUploading> MockListDSTNetSalaryUploading()
        {
            return new List<DSTNetSalaryUploading>
            {
                new DSTNetSalaryUploading {
                    RowNumber = 8,
                    EmployeeCode = "111",
                    Currency = "VND",
                    NetSalaryReceived = "99999999"
                }
            };
        }

        private static Failure<AttendanceTemplate> MockFailureResultAttendanceTemplate()
        {
            return new Failure<AttendanceTemplate>(Message.IncorrectTemplate);
        }

        private static Failure<DSTNetSalaryTemplate> MockFailureResultDSTNetSalaryTemplate()
        {
            return new Failure<DSTNetSalaryTemplate>(Message.IncorrectTemplate);
        }

        private IEnumerable<ValidationResult> MockListValidationResult()
        {
            return new List<ValidationResult>
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

        #endregion Initialization
    }
}