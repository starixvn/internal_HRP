namespace HRP.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Criteria.RefResource;
    using HRP.Core.Enumerations;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.Employee;
    using HRP.Models;
    using HRP.Models.EmployeeManagement;
    using HRP.Repositories;
    using HRP.Resources;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using NSubstitute;
    using Xunit;

    public class EmployeeServiceTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private ICryptoService cryptoService;
        private IEmployeeRepository employeeRepository;

        #region GetDataFilterForIndexPageAsync

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task GetDataFilterForIndexPageAsync_UserIdIsInvalid_ReturnDefaultViewModel(int userId)
        {
            // Arrange
            var employeeService = GetEmployeeService();
            sessionService.UserId = userId;

            // Act
            var actual = await employeeService.GetDataFilterForIndexPageAsync(Arg.Any<string>(), Arg.Any<string>());

            // Assert
            Assert.IsType<EmployeeManagementViewModel>(actual);
            Assert.False(actual.IsHostCreateEmployee);
            Assert.Null(actual.DepartmentIds);
            Assert.Null(actual.EmploymentStatusIds);
            Assert.Null(actual.EmployeeId);
            Assert.Null(actual.DisplayName);
            Assert.Empty(actual.HierarchyCompanyJson);
            Assert.Empty(actual.EmploymentStatusJson);
            Assert.Empty(actual.Employees);
            Assert.Empty(actual.HierarchyCompanies);
            Assert.Empty(actual.EmploymentStatuses);
        }

        [Fact]
        public async Task GetDataFilterForIndexPageAsync_UserIdIsValid_ReturnAsExpectedViewModel()
        {
            // Arrange
            var viewModel = new EmployeeManagementViewModel
            {
                IsHostCreateEmployee = true,
                DepartmentIds = "1,2"
            };
            var employeeService = GetEmployeeService();
            employeeRepository.GetDataFilterForIndexPageAsync(Arg.Any<GetDataFilterForIndexPageCriteria>()).Returns(viewModel);

            // Act
            var actual = await employeeService.GetDataFilterForIndexPageAsync(Arg.Any<string>(), Arg.Any<string>());

            // Assert
            Assert.Equal(viewModel, actual);
        }

        #endregion GetDataFilterForIndexPageAsync

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

        #region GetPersonalDetails

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GetPersonalDetails_EmployeeIdIsInvalid_ReturnNull(int employeeId)
        {
            // Arrange
            var employeeService = GetEmployeeService();

            // Act
            var actual = employeeService.GetPersonalDetails(employeeId);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public void GetPersonalDetails_EmployeeIdIsValid_ReturnAsExpectedViewModel()
        {
            // Arrange
            var expected = new CreateEmployeeViewModel();
            var employeeService = GetEmployeeService();
            employeeRepository.GetPersonalDetails(Arg.Any<GetEmployeeByIdCriteria>()).Returns(expected);

            // Act
            var actual = employeeService.GetPersonalDetails(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion GetPersonalDetails

        #region GetDataToCreateEmployeeAsync

        [Fact]
        public async Task GetDataToCreateEmployeeAsync_WhenCalled_ReturnAsExpectedViewModel()
        {
            // Arrange
            var expected = new List<ReferenceResourceModel>();
            var employeeService = GetEmployeeService();
            baseService.GetDataAsync<ReferenceResourceModel>(Arg.Any<GetRefResourceByNamesCriteria>()).Returns(expected);

            // Act
            var actual = await employeeService.GetDataToCreateEmployeeAsync();

            // Assert
            Assert.IsType<CreateEmployeeViewModel>(actual);
            Assert.Equal(0, actual.EmployeeId);
            Assert.Null(actual.FullName);
            Assert.Null(actual.Nickname);
            Assert.Equal(0, actual.Gender);
            Assert.Equal(DateTime.MinValue, actual.DateOfBirth);
            Assert.Null(actual.PlaceOfBirth);
            Assert.Equal(0, actual.Nationality);
            Assert.Null(actual.NRICNo);
            Assert.Equal(0, actual.MaritalStatus);
            Assert.Equal(0, actual.NoOfChildren);
            Assert.Null(actual.OverseasPhoneNo);
            Assert.Null(actual.LocalPhoneNo);
            Assert.Null(actual.OtherPhoneNo);
            Assert.Null(actual.PassportNo);
            Assert.Equal(DateTime.MinValue, actual.DateIssue);
            Assert.Equal(DateTime.MinValue, actual.ExpiredDate);
            Assert.Null(actual.PlaceOfIssue);
            Assert.Null(actual.Skype);
            Assert.Null(actual.PersonalEmail);
            Assert.Null(actual.PermanentAddress);
            Assert.Null(actual.ResidenceAddress);
            Assert.Null(actual.MailingAddress);
            Assert.Null(actual.ProfilePic);
            Assert.Null(actual.ProfilePicture);
            Assert.False(actual.IsHost);
            Assert.Equal(0, actual.EmergencyContact.EmployeeId);
            Assert.Null(actual.EmergencyContact.Name);
            Assert.Null(actual.EmergencyContact.Relationship);
            Assert.Null(actual.EmergencyContact.Address);
            Assert.Null(actual.EmergencyContact.PhoneNo1);
            Assert.Null(actual.EmergencyContact.PhoneNo2);
            Assert.Empty(actual.GenderList);
            Assert.Empty(actual.NationalityList);
            Assert.Empty(actual.MaritalStatusList);
            Assert.Empty(actual.NoOfChildrenList);
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

        #region GetDataToCreateEmployment

        [Fact]
        public void GetDataToCreateEmployment_UserIdIsInvalid_ReturnDefaultModel()
        {
            // Arrange
            var expected = new CreateEmploymentViewModel();
            var employeeService = GetEmployeeService();
            sessionService.UserId = -1;

            // Act
            var actual = employeeService.GetDataToCreateEmployment();

            // Assert
            Assert.Equal(expected.CompanyId, actual.CompanyId);
            Assert.Equal(expected.LocalCompanyId, actual.LocalCompanyId);
            Assert.Equal(expected.JobTitle, actual.JobTitle);
            Assert.Equal(expected.LocalJobTitleId, actual.LocalJobTitleId);
            Assert.Equal(expected.GroupStartDate, actual.GroupStartDate);
            Assert.Equal(expected.GroupEndDate, actual.GroupEndDate);
            Assert.Equal(expected.CompanyStartDate, actual.CompanyStartDate);
            Assert.Equal(expected.CompanyEndDate, actual.CompanyEndDate);
            Assert.Equal(expected.CompanyCode, actual.CompanyCode);
            Assert.Equal(expected.EmployeeCode, actual.EmployeeCode);
            Assert.Equal(expected.EmploymentType, actual.EmploymentType);
            Assert.Equal(expected.EmploymentStatus, actual.EmploymentStatus);
            Assert.Equal(expected.JobCategoryId, actual.JobCategoryId);
            Assert.Equal(expected.GradeLevelId, actual.GradeLevelId);
            Assert.Equal(expected.JobGradeId, actual.JobGradeId);
            Assert.Equal(expected.DivisionId, actual.DivisionId);
            Assert.Equal(expected.DepartmentId, actual.DepartmentId);
            Assert.Equal(expected.CostCenterId, actual.CostCenterId);
            Assert.Equal(expected.SectionId, actual.SectionId);
            Assert.Equal(expected.Email, actual.Email);
            Assert.Equal(expected.CountryId, actual.CountryId);
            Assert.Equal(expected.CityId, actual.CityId);
            Assert.Equal(expected.OfficeId, actual.OfficeId);
            Assert.Equal(expected.WorkPermit.LocalCompanyId, actual.WorkPermit.LocalCompanyId);
            Assert.Equal(expected.WorkPermit.Type1, actual.WorkPermit.Type1);
            Assert.Equal(expected.WorkPermit.StartDateType1, actual.WorkPermit.StartDateType1);
            Assert.Equal(expected.WorkPermit.EndDateType1, actual.WorkPermit.EndDateType1);
            Assert.Equal(expected.WorkPermit.StartDateType2, actual.WorkPermit.StartDateType2);
            Assert.Equal(expected.WorkPermit.EndDateType2, actual.WorkPermit.EndDateType2);
            Assert.Equal(expected.DST, actual.DST);
            Assert.Equal(expected.DBT, actual.DBT);
            Assert.Equal(expected.Companies, actual.Companies);
        }

        [Fact]
        public void GetDataToCreateEmployment_UserIdIsValid_ReturnExpectedModel()
        {
            // Arrange
            var expected = new CreateEmploymentViewModel
            {
                Companies = new List<Company> { new Company { CompanyId = 1, Code = "SMS" } }
            };
            var employeeService = GetEmployeeService();
            employeeRepository.GetDataToCreateEmployment(Arg.Any<GetDataToCreateEmploymentCriteria>()).Returns(expected);

            // Act
            var actual = employeeService.GetDataToCreateEmployment();

            // Assert
            Assert.Equal(expected.CompanyId, actual.CompanyId);
            Assert.Equal(expected.LocalCompanyId, actual.LocalCompanyId);
            Assert.Equal(expected.JobTitle, actual.JobTitle);
            Assert.Equal(expected.LocalJobTitleId, actual.LocalJobTitleId);
            Assert.Equal(expected.GroupStartDate, actual.GroupStartDate);
            Assert.Equal(expected.GroupEndDate, actual.GroupEndDate);
            Assert.Equal(expected.CompanyStartDate, actual.CompanyStartDate);
            Assert.Equal(expected.CompanyEndDate, actual.CompanyEndDate);
            Assert.Equal(expected.CompanyCode, actual.CompanyCode);
            Assert.Equal(expected.EmployeeCode, actual.EmployeeCode);
            Assert.Equal(expected.EmploymentType, actual.EmploymentType);
            Assert.Equal(expected.EmploymentStatus, actual.EmploymentStatus);
            Assert.Equal(expected.JobCategoryId, actual.JobCategoryId);
            Assert.Equal(expected.GradeLevelId, actual.GradeLevelId);
            Assert.Equal(expected.JobGradeId, actual.JobGradeId);
            Assert.Equal(expected.DivisionId, actual.DivisionId);
            Assert.Equal(expected.DepartmentId, actual.DepartmentId);
            Assert.Equal(expected.CostCenterId, actual.CostCenterId);
            Assert.Equal(expected.SectionId, actual.SectionId);
            Assert.Equal(expected.Email, actual.Email);
            Assert.Equal(expected.CountryId, actual.CountryId);
            Assert.Equal(expected.CityId, actual.CityId);
            Assert.Equal(expected.OfficeId, actual.OfficeId);
            Assert.Equal(expected.WorkPermit.LocalCompanyId, actual.WorkPermit.LocalCompanyId);
            Assert.Equal(expected.WorkPermit.Type1, actual.WorkPermit.Type1);
            Assert.Equal(expected.WorkPermit.StartDateType1, actual.WorkPermit.StartDateType1);
            Assert.Equal(expected.WorkPermit.EndDateType1, actual.WorkPermit.EndDateType1);
            Assert.Equal(expected.WorkPermit.StartDateType2, actual.WorkPermit.StartDateType2);
            Assert.Equal(expected.WorkPermit.EndDateType2, actual.WorkPermit.EndDateType2);
            Assert.Equal(expected.DST, actual.DST);
            Assert.Equal(expected.DBT, actual.DBT);
            Assert.Equal(expected.Companies, actual.Companies);
        }

        #endregion GetDataToCreateEmployment

        #region GetDataToUpdateEmployment

        [Fact]
        public void GetDataToUpdateEmployment_EmployeeIdIsInvalid_ReturnNull()
        {
            // Arrange
            var employeeService = GetEmployeeService();

            // Act
            var actual = employeeService.GetDataToUpdateEmployment(-1);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public void GetDataToUpdateEmployment_EmployeeIdIsValid_ReturnExpectedModel()
        {
            // Arrange
            var pdfPassword = "1234";
            var isHost = true;
            var expected = new UpdateEmploymentViewModel();
            var employeeService = GetEmployeeService();
            employeeRepository.GetDataToUpdateEmployment(Arg.Any<GetDataToUpdateEmploymentCriteria>()).Returns(expected);
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(isHost);
            cryptoService.Decrypt(Arg.Any<string>()).Returns(pdfPassword);

            // Act
            var actual = employeeService.GetDataToUpdateEmployment(1);

            // Assert
            Assert.Equal(expected.CompanyId, actual.CompanyId);
            Assert.Equal(expected.LocalCompanyId, actual.LocalCompanyId);
            Assert.Equal(expected.JobTitle, actual.JobTitle);
            Assert.Equal(expected.LocalJobTitleId, actual.LocalJobTitleId);
            Assert.Equal(expected.GroupStartDate, actual.GroupStartDate);
            Assert.Equal(expected.GroupEndDate, actual.GroupEndDate);
            Assert.Equal(expected.CompanyStartDate, actual.CompanyStartDate);
            Assert.Equal(expected.CompanyEndDate, actual.CompanyEndDate);
            Assert.Equal(expected.CompanyCode, actual.CompanyCode);
            Assert.Equal(expected.EmployeeCode, actual.EmployeeCode);
            Assert.Equal(expected.EmploymentType, actual.EmploymentType);
            Assert.Equal(expected.EmploymentStatus, actual.EmploymentStatus);
            Assert.Equal(expected.JobCategoryId, actual.JobCategoryId);
            Assert.Equal(expected.GradeLevelId, actual.GradeLevelId);
            Assert.Equal(expected.JobGradeId, actual.JobGradeId);
            Assert.Equal(expected.DivisionId, actual.DivisionId);
            Assert.Equal(expected.DepartmentId, actual.DepartmentId);
            Assert.Equal(expected.CostCenterId, actual.CostCenterId);
            Assert.Equal(expected.SectionId, actual.SectionId);
            Assert.Equal(expected.Email, actual.Email);
            Assert.Equal(expected.CountryId, actual.CountryId);
            Assert.Equal(expected.CityId, actual.CityId);
            Assert.Equal(expected.OfficeId, actual.OfficeId);
            Assert.Equal(expected.WorkPermit.LocalCompanyId, actual.WorkPermit.LocalCompanyId);
            Assert.Equal(expected.WorkPermit.Type1, actual.WorkPermit.Type1);
            Assert.Equal(expected.WorkPermit.StartDateType1, actual.WorkPermit.StartDateType1);
            Assert.Equal(expected.WorkPermit.EndDateType1, actual.WorkPermit.EndDateType1);
            Assert.Equal(expected.WorkPermit.StartDateType2, actual.WorkPermit.StartDateType2);
            Assert.Equal(expected.WorkPermit.EndDateType2, actual.WorkPermit.EndDateType2);
            Assert.Equal(expected.DST, actual.DST);
            Assert.Equal(expected.DBT, actual.DBT);
            Assert.Equal(isHost, actual.IsHost);
            Assert.Equal(pdfPassword, actual.PDFPassword);
        }

        #endregion GetDataToUpdateEmployment

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

        private static ISessionService FakeSessionService()
        {
            var sessionServiceInstance = Substitute.For<ISessionService>();
            sessionServiceInstance.UserId = 99;

            return sessionServiceInstance;
        }

        private static ICryptoService FakeCryptoService()
        {
            return Substitute.For<ICryptoService>();
        }

        private static IHostingEnvironment FakeHostingEnvironment()
        {
            return Substitute.For<IHostingEnvironment>();
        }

        private static IEmployeeRepository FakeEmployeeRepository()
        {
            return Substitute.For<IEmployeeRepository>();
        }

        private EmployeeService GetEmployeeService()
        {
            var hostingEnviroment = FakeHostingEnvironment();
            sessionService = FakeSessionService();
            baseService = FakeBaseService();
            employeeRepository = FakeEmployeeRepository();
            cryptoService = FakeCryptoService();
            var service = new EmployeeService(hostingEnviroment, baseService, cryptoService, sessionService, employeeRepository);

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

        #endregion Helper
    }
}