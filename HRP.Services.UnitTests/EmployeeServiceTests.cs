namespace HRP.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.EmergencyContact;
    using HRP.Criteria.Employee;
    using HRP.Models.Employee;
    using HRP.Repositories;
    using HRP.Resources;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NSubstitute;
    using Xunit;

    public class EmployeeServiceTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private ICryptoService cryptoService;
        private ICommonService commonService;
        private IEmployeeRepository employeeRepository;

        [Fact]
        public async Task GetEmployeeBasicInfoByIdAsync_WhenCalled_ReturnAsExpectedEmployee()
        {
            // Arrange
            var employee = new EmployeeBasicInfo
            {
                EmployeeId = 1,
                EmployeeCode = "111",
                FullName = "trinh dang thang"
            };
            var employeeService = GetEmployeeService();
            baseService.GetDataEntityAsync<EmployeeBasicInfo>(Arg.Any<GetEmployeeBasicInfoByIdCriteria>()).Returns(employee);

            // Act
            var actual = await employeeService.GetEmployeeBasicInfoByIdAsync(1);

            // Assert
            Assert.Equal(employee, actual);
        }

        #region GetEmployeesAsync

        [Fact]
        public async Task GetEmployeesAsync_WhenCalled_ReturnAsExpectedEmployees()
        {
            // Arrange
            var employees = new List<EmployeeBasicInfo>
            {
                new EmployeeBasicInfo
                {
                    EmployeeId = 1,
                    EmployeeCode = "111",
                    Nickname = "tim.trinh",
                    FullName = "trinh dang thang",
                    CompanyCode = "DAR",
                    DivisionCode = "SM",
                    DepartmentCode = "SM",
                    JobTitle = "Management",
                    EmployeeStatus = "Confirmed",
                    EditBankPolicy = Policy.Read,
                    EditPayRollPolicy = Policy.Read,
                    EditProfilePolicy = Policy.Read
                }
            };
            var employeeService = GetEmployeeService();
            baseService.GetDataAsync<EmployeeBasicInfo>(Arg.Any<GetAllEmployeeCriteria>()).Returns(employees);

            // Act
            var actual = await employeeService.GetEmployeesAsync(new EmployeeManagementViewModel());

            // Assert
            Assert.Equal(employees, actual);
        }

        #endregion GetEmployeesAsync

        #region CreateEmployeeAsync

        [Fact]
        public async Task CreateEmployeeAsync_ProfilePictureIsNull_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var viewModel = new CreateEmploymentViewModel();
            var employeeService = GetEmployeeService();
            cryptoService.Encrypt(Arg.Any<string>()).Returns("aa");
            baseService.ExecuteNonQueryAsync(Arg.Any<CreateEmployeeCriteria>()).Returns(expected);

            // Act
            var actual = await employeeService.CreateEmployeeAsync(viewModel);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateEmployeeAsync_CreatedSuccessfullyAndProfilePictureIsNotNull_ExpectedSavePicture()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0,
                Message = "Successful!"
            };
            var viewModel = new CreateEmploymentViewModel { ProfilePicture = FakePictureProfile() };
            var employeeService = GetEmployeeService();
            cryptoService.Encrypt(Arg.Any<string>()).Returns("aa");
            baseService.ExecuteNonQueryAsync(Arg.Any<CreateEmployeeCriteria>()).Returns(expected);

            // Act
            var actual = await employeeService.CreateEmployeeAsync(viewModel);

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion CreateEmployeeAsync

        #region UpdatePersonalDetailsAsync

        [Fact]
        public async Task UpdatePersonalDetailsAsync_ProfilePictureIsNull_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 1,
                Message = "Successful!"
            };
            var viewModel = new CreateEmployeeViewModel();
            var employeeService = GetEmployeeService();
            baseService.ExecuteNonQueryAsync(Arg.Any<UpdateEmployeeCriteria>()).Returns(expected);

            // Act
            var actual = await employeeService.UpdatePersonalDetailsAsync(viewModel);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task UpdatePersonalDetailsAsync_UpdatedSuccessfullyAndProfilePictureIsNotNull_ExpectedSavePicture()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0,
                Message = "Successful!"
            };
            var viewModel = new CreateEmployeeViewModel { ProfilePicture = FakePictureProfile() };
            var employeeService = GetEmployeeService();
            baseService.ExecuteNonQueryAsync(Arg.Any<UpdateEmployeeCriteria>()).Returns(expected);

            // Act
            var actual = await employeeService.UpdatePersonalDetailsAsync(viewModel);

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion UpdatePersonalDetailsAsync

        #region GetEmployeeByIdAsync

        [Fact]
        public async Task GetEmployeeByIdAsync_WhenCalled_ReturnExpectedEmployee()
        {
            // Arrange
            var employeeId = 11;
            var employee = MockEmployee();
            var employeeService = GetEmployeeService();
            baseService.GetDataEntityAsync<Employee>
                (Arg.Is<GetEmployeeByIdCriteria>(x => x.EmployeeId == employeeId && x.UserId == sessionService.UserId && x.FunctionKey == FunctionKey.EmployeeProfile))
                .Returns(employee);

            // Act
            var actual = await employeeService.GetEmployeeByIdAsync(employeeId);

            // Assert
            Assert.Equal(employee, actual);
        }

        #endregion GetEmployeeByIdAsync

        #region GetEmergencyContactByEmployeeIdAsync

        [Fact]
        public async Task GetEmergencyContactByEmployeeIdAsync_WhenCalled_ReturnExpectedEmergencyContact()
        {
            // Arrange
            var employeeId = 11;
            var contact = new EmergencyContact
            {
                EmployeeId = 11,
                Address = "asdasd"
            };
            var employeeService = GetEmployeeService();
            baseService.GetDataEntityAsync<EmergencyContact>
                (Arg.Is<GetEmergencyContactByEmployeeIdCriteria>(x => x.EmployeeId == employeeId))
                .Returns(contact);

            // Act
            var actual = await employeeService.GetEmergencyContactByEmployeeIdAsync(employeeId);

            // Assert
            Assert.Equal(contact, actual);
        }

        #endregion GetEmergencyContactByEmployeeIdAsync

        #region GetDataToCreateEmployeeAsync

        [Fact]
        public async Task GetDataToCreateEmployeeAsync_WhenCalled_ReturnAsExpectedTuple()
        {
            // Arrange
            var refResources = new List<ReferenceResourceModel>
            {
                new ReferenceResourceModel {RefName = ReferenceResource.Gender, Value = "a", ResourceId = 1 },
                new ReferenceResourceModel {RefName = ReferenceResource.Nationality, Value = "b", ResourceId = 11 },
                new ReferenceResourceModel {RefName = ReferenceResource.MaritalStatus, Value = "c", ResourceId = 2 },
                new ReferenceResourceModel {RefName = ReferenceResource.NoOfChildren, Value = "d", ResourceId = 33 }
            };
            var expected = new SelectList(refResources, "ResourceId", "Value");

            var employeeService = GetEmployeeService();
            commonService.GetReferenceResourcesByNamesAsync(Arg.Any<string[]>()).Returns(refResources);
            commonService.FilterReferenceResources(Arg.Any<IEnumerable<ReferenceResourceModel>>(), Arg.Any<string>()).Returns(refResources);
            commonService.GetSelectList(Arg.Any<IEnumerable<ReferenceResourceModel>>()).Returns(expected);

            // Act
            var actual = await employeeService.GetDataToCreateEmployeeAsync();

            // Assert
            Assert.IsType<Tuple<SelectList, SelectList, SelectList, SelectList>>(actual);
            Assert.Equal(expected, actual.Item1);
            Assert.Equal(expected, actual.Item2);
            Assert.Equal(expected, actual.Item3);
            Assert.Equal(expected, actual.Item4);
        }

        #endregion GetDataToCreateEmployeeAsync

        #region CheckValidPicture

        [Fact]
        public void CheckValidPicture_NullParam_ReturnExpectedCommandResult()
        {
            // Arrange
            var employeeService = GetEmployeeService();

            // Act
            var actual = employeeService.CheckValidPicture(null);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(0, actual.Code);
            Assert.Equal(string.Empty, actual.Message);
        }

        [Fact]
        public void CheckValidPicture_LargeFileSize_ReturnExpectedCommandResult()
        {
            // Arrange
            var fileSize = (1024 * 1024) * 5;
            var employeeService = GetEmployeeService();

            // Act
            var actual = employeeService.CheckValidPicture(FakePictureProfile(fileSize));

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(1, actual.Code);
            Assert.Equal(Message.FileTooLarge, actual.Message);
        }

        [Fact]
        public void CheckValidPicture_InvalidImageType_ReturnExpectedCommandResult()
        {
            // Arrange
            var employeeService = GetEmployeeService();

            // Act
            var actual = employeeService.CheckValidPicture(FakeTxtFile());

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(1, actual.Code);
            Assert.Equal(Message.InvalidFileType, actual.Message);
        }

        [Fact]
        public void CheckValidPicture_WithValidImage_ReturnExpectedCommandResult()
        {
            // Arrange
            var employeeService = GetEmployeeService();

            // Act
            var actual = employeeService.CheckValidPicture(FakePictureProfile());

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(0, actual.Code);
            Assert.Equal(string.Empty, actual.Message);
        }

        #endregion CheckValidPicture

        #region UpdateEmploymentAsync

        [Fact]
        public async Task UpdateEmploymentAsync_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var model = new UpdateEmploymentViewModel();
            var expected = new CommandResult();
            var employeeService = GetEmployeeService();
            baseService.ExecuteNonQueryAsync(Arg.Any<UpdateEmploymentCriteria>()).Returns(expected);

            // Act
            var actual = await employeeService.UpdateEmploymentAsync(model);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task UpdateEmploymentAsync_WhenCalled_ExpectedCallEncrypt()
        {
            // Arrange
            var model = new UpdateEmploymentViewModel();
            var expected = new CommandResult();
            var employeeService = GetEmployeeService();
            baseService.ExecuteNonQueryAsync(Arg.Any<UpdateEmploymentCriteria>()).Returns(expected);

            // Act
            var actual = await employeeService.UpdateEmploymentAsync(model);

            // Assert
            cryptoService.Received().Encrypt(Arg.Any<string>());
        }

        #endregion UpdateEmploymentAsync

        #region Helper

        private static IBaseService FakeBaseService()
        {
            return Substitute.For<IBaseService>();
        }

        private ISessionService FakeSessionService()
        {
            var sessionServiceInstance = Substitute.For<ISessionService>();
            sessionServiceInstance.UserId = 99;

            return sessionServiceInstance;
        }

        private ICryptoService FakeCryptoService()
        {
            return Substitute.For<ICryptoService>();
        }

        private IHostingEnvironment FakeHostingEnvironment()
        {
            return Substitute.For<IHostingEnvironment>();
        }

        private IEmployeeRepository FakeEmployeeRepository()
        {
            return Substitute.For<IEmployeeRepository>();
        }

        private ICommonService FakeCommonService()
        {
            return Substitute.For<ICommonService>();
        }

        private EmployeeService GetEmployeeService()
        {
            var hostingEnviroment = FakeHostingEnvironment();
            sessionService = FakeSessionService();
            baseService = FakeBaseService();
            employeeRepository = FakeEmployeeRepository();
            cryptoService = FakeCryptoService();
            commonService = FakeCommonService();
            var service = new EmployeeService(hostingEnviroment, baseService, cryptoService, sessionService, commonService, employeeRepository);

            return service;
        }

        private static IFormFile FakePictureProfile(long fileSize = (1024 * 1024), string contentType = "image/png")
        {
            var file = Substitute.For<IFormFile>();
            string sourceImg = Path.GetFullPath(@"profile.png");
            var fileName = "profile.png";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(sourceImg);
            writer.Flush();
            ms.Position = 0;
            file.OpenReadStream().Returns(ms);
            file.FileName.Returns(fileName);
            file.Length.Returns(fileSize);
            file.ContentType.Returns(contentType);

            return file;
        }

        private static IFormFile FakeTxtFile()
        {
            var file = Substitute.For<IFormFile>();
            var fileName = "test.txt";
            file.FileName.Returns(fileName);
            file.Length.Returns(1024);

            return file;
        }

        private Employee MockEmployee()
        {
            return new Employee
            {
                EmployeeId = 12,
                FullName = "full",
                Nickname = "nick"
            };
        }

        #endregion Helper
    }
}