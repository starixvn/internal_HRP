namespace HRP.Services.UnitTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.Data;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.Employee;
    using HRP.Models;
    using HRP.Models.EmployeeManagement;
    using HRP.Repositories.Interfaces;
    using HRP.Resources;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NSubstitute;
    using Xunit;

    public class EmployeeManagementServiceTests
    {
        private readonly IObjectDb mockObjectDb;
        private readonly Fixture fixture;
        private IBaseService baseService;
        private ISessionService sessionService;
        private ICryptoService cryptoService;
        private IEmployeeManagementRepository employeeRepository;

        public EmployeeManagementServiceTests()
        {
            DbSettingProviderManager
            .StartTestingSession()
            .Run();

            mockObjectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.Mock((key, time) => mockObjectDb);

            fixture = InitFixture();
        }

        #region CreateEmployeeAsync

        [Fact]
        public async Task CreateEmployeeAsync_WithValidCreateEmployeeViewModel_ReturnCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0,
                Message = "Successful!"
            };
            var criteria = new CreateEmployeeCriteria();

            var viewModel = FakeCreateEmployeeViewModel();
            var employeeService = GetEmployeeManagementService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateEmployeeCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await employeeService.CreateEmployeeAsync(viewModel);

            // Assert
            Assert.Equal(expected, actual);
            Assert.Equal(criteria.FullName, viewModel.FullName);
            Assert.Equal(criteria.NickName, viewModel.NickName);
            Assert.Equal(criteria.Gender, viewModel.Gender);
            Assert.Equal(criteria.DateOfBirth, viewModel.DateOfBirth);
            Assert.Equal(criteria.PlaceOfBirth, viewModel.PlaceOfBirth);
            Assert.Equal(criteria.Nationality, viewModel.Nationality);
            Assert.Equal(criteria.NRICNo, viewModel.NRICNo);
            Assert.Equal(criteria.MaritalStatus, viewModel.MaritalStatus);
            Assert.Equal(criteria.NoOfChildren, viewModel.NoOfChildren);
            Assert.Equal(criteria.OverseasPhoneNo, viewModel.OverseasPhoneNo);
            Assert.Equal(criteria.LocalPhoneNo, viewModel.LocalPhoneNo);
            Assert.Equal(criteria.OtherPhoneNo, viewModel.OtherPhoneNo);
            Assert.Equal(criteria.PassportNo, viewModel.PassportNo);
            Assert.Equal(criteria.DateIssue, viewModel.DateIssue);
            Assert.Equal(criteria.ExpiredDate, viewModel.ExpiredDate);
            Assert.Equal(criteria.PlaceOfIssue, viewModel.PlaceOfIssue);
            Assert.Equal(criteria.Skype, viewModel.Skype);
            Assert.Equal(criteria.PersonalEmail, viewModel.PersonalEmail);
            Assert.Equal(criteria.PermanentAddress, viewModel.PermanentAddress);
            Assert.Equal(criteria.ResidenceAddress, viewModel.ResidenceAddress);
            Assert.Equal(criteria.MailingAddress, viewModel.MailingAddress);
            Assert.Equal(criteria.ECName, viewModel.EmergencyContact.Name);
            Assert.Equal(criteria.ECRelationship, viewModel.EmergencyContact.Relationship);
            Assert.Equal(criteria.ECAddress, viewModel.EmergencyContact.Address);
            Assert.Equal(criteria.ECPhoneNo1, viewModel.EmergencyContact.PhoneNo1);
            Assert.Equal(criteria.ECPhoneNo2, viewModel.EmergencyContact.PhoneNo2);
        }

        [Fact]
        public async Task CreateEmployeeAsync_CreateEmployeeViewModelHasValueProfilePicture_SavePictureAsync()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0,
                Message = "Successful!"
            };
            var viewModel = FakeCreateEmployeeViewModel();
            viewModel.ProfilePicture = FakePictureProfile();
            var employeeService = GetEmployeeManagementService();
            baseService.ExecuteNonQueryAsync(Arg.Any<CreateEmployeeCriteria>()).Returns(expected);

            // Act
            var actual = await employeeService.CreateEmployeeAsync(viewModel);

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion CreateEmployeeAsync

        #region UpdatePersonalDetailsAsync

        [Fact]
        public async Task UpdatePersonalDetailsAsync_WithValidUpdateEmployeeCriteria_ReturnCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0,
                Message = "Successful!"
            };
            var criteria = fixture.Create<UpdateEmployeeCriteria>();
            var viewModel = FakeCreateEmployeeViewModel();
            var employeeService = GetEmployeeManagementService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateEmployeeCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await employeeService.UpdatePersonalDetailsAsync(viewModel);

            // Assert
            Assert.Equal(expected, actual);
            Assert.Equal(criteria.FullName, viewModel.FullName);
            Assert.Equal(criteria.NickName, viewModel.NickName);
            Assert.Equal(criteria.Gender, viewModel.Gender);
            Assert.Equal(criteria.DateOfBirth, viewModel.DateOfBirth);
            Assert.Equal(criteria.PlaceOfBirth, viewModel.PlaceOfBirth);
            Assert.Equal(criteria.Nationality, viewModel.Nationality);
            Assert.Equal(criteria.NRICNo, viewModel.NRICNo);
            Assert.Equal(criteria.MaritalStatus, viewModel.MaritalStatus);
            Assert.Equal(criteria.NoOfChildren, viewModel.NoOfChildren);
            Assert.Equal(criteria.OverseasPhoneNo, viewModel.OverseasPhoneNo);
            Assert.Equal(criteria.LocalPhoneNo, viewModel.LocalPhoneNo);
            Assert.Equal(criteria.OtherPhoneNo, viewModel.OtherPhoneNo);
            Assert.Equal(criteria.PassportNo, viewModel.PassportNo);
            Assert.Equal(criteria.DateIssue, viewModel.DateIssue);
            Assert.Equal(criteria.ExpiredDate, viewModel.ExpiredDate);
            Assert.Equal(criteria.PlaceOfIssue, viewModel.PlaceOfIssue);
            Assert.Equal(criteria.Skype, viewModel.Skype);
            Assert.Equal(criteria.PersonalEmail, viewModel.PersonalEmail);
            Assert.Equal(criteria.PermanentAddress, viewModel.PermanentAddress);
            Assert.Equal(criteria.ResidenceAddress, viewModel.ResidenceAddress);
            Assert.Equal(criteria.MailingAddress, viewModel.MailingAddress);
            Assert.Equal(criteria.ECName, viewModel.EmergencyContact.Name);
            Assert.Equal(criteria.ECRelationship, viewModel.EmergencyContact.Relationship);
            Assert.Equal(criteria.ECAddress, viewModel.EmergencyContact.Address);
            Assert.Equal(criteria.ECPhoneNo1, viewModel.EmergencyContact.PhoneNo1);
            Assert.Equal(criteria.ECPhoneNo2, viewModel.EmergencyContact.PhoneNo2);
        }

        [Fact]
        public async Task UpdatePersonalDetailsAsync_CreateEmployeeViewModelHasValueProfilePicture_SavePictureAsync()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0,
                Message = "Successful!"
            };
            var viewModel = FakeCreateEmployeeViewModel();
            viewModel.ProfilePicture = FakePictureProfile();
            var employeeService = GetEmployeeManagementService();
            baseService.ExecuteNonQueryAsync(Arg.Any<UpdateEmployeeCriteria>()).Returns(expected);

            // Act
            var actual = await employeeService.UpdatePersonalDetailsAsync(viewModel);

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion UpdatePersonalDetailsAsync

        #region GetAllCompanyDivisonDepartment

        [Fact]
        public void GetAllCompanyDivisionDepartment_ValidCriteria_ReturnList()
        {
            // Arrange
            var employeeService = GetEmployeeManagementService();

            // Act
            var actual = employeeService.GetAllCompanyDivisonDepartment();

            //Assert
            Assert.NotNull(actual);
            Assert.IsType<ListCompanyDivisionDepartment>(actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GetAllCompanyDivisionDepartment_InvalidCriteria_ReturnNull(int userId)
        {
            //Arrange
            var employeeService = GetEmployeeManagementService();
            sessionService.UserId = userId;

            //Act
            var actual = employeeService.GetAllCompanyDivisonDepartment();

            //Assert
            Assert.Null(actual);
        }

        #endregion GetAllCompanyDivisonDepartment

        #region GetEmployeeById

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GetEmployeeById_WithInvalidEmployeeId_ReturnNull(int employeeId)
        {
            // Arrange
            var employeeService = GetEmployeeManagementService();

            // Act
            var actual = employeeService.GetEmployeeById(employeeId);

            // Assert
            Assert.Null(actual);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(99)]
        public void GetEmployeeById_WithValidEmployeeId_ReturnViewModel(int employeeId)
        {
            // Arrange
            var expected = FakeCreateEmployeeViewModel();
            var employeeService = GetEmployeeManagementService();
            var reader = Substitute.For<IMultiResultReader>();
            reader.Read<CreateEmployeeViewModel>().Returns(new List<CreateEmployeeViewModel> { expected });
            mockObjectDb.QueryMultiple(Arg.Is<GetEmployeeByIdCriteria>(x => x.EmployeeId == employeeId)).Returns(reader);

            // Act
            var actual = employeeService.GetEmployeeById(employeeId);

            // Assert
            Assert.Equal(expected, actual);
            Assert.Equal(expected.ProfilePicLink, actual.ProfilePicLink);
        }

        #endregion GetEmployeeById

        #region InitDataForNewEmployeeAsync

        [Fact]
        public async Task InitDataForNewEmployeeAsync_WhenCall_ReturnCreateEmployeeViewModelAsync()
        {
            // Arrange
            var expected = fixture.Create<IEnumerable<ReferenceResourceModel>>();
            var employeeService = GetEmployeeManagementService();
            baseService.GetDataAsync<ReferenceResourceModel>(Arg.Any<GetRefResourceByNamesCriteria>()).Returns(expected);

            // Act
            var actual = await employeeService.InitDataForNewEmployeeAsync();

            // Assert
            Assert.IsType<CreateEmployeeViewModel>(actual);
        }

        #endregion InitDataForNewEmployeeAsync

        #region CheckValidPicture

        [Fact]
        public void CheckValidPicture_NullParam_ReturnExpectedCommandResult()
        {
            // Arrange
            var employeeService = GetEmployeeManagementService();

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
            var employeeService = GetEmployeeManagementService();

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
            var employeeService = GetEmployeeManagementService();

            // Act
            var actual = employeeService.CheckValidPicture(FakeTxtFile());

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(1, actual.Code);
            Assert.Equal(Message.InvalidImageType, actual.Message);
        }

        [Fact]
        public void CheckValidPicture_WithValidImage_ReturnExpectedCommandResult()
        {
            // Arrange
            var employeeService = GetEmployeeManagementService();

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
            var employeeService = GetEmployeeManagementService();
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
            var employeeService = GetEmployeeManagementService();
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
            var employeeService = GetEmployeeManagementService();

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
            var employeeService = GetEmployeeManagementService();
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

        private static IEmployeeManagementRepository FakeEmployeeRepository()
        {
            return Substitute.For<IEmployeeManagementRepository>();
        }

        private EmployeeManagementService GetEmployeeManagementService()
        {
            var hostingEnviroment = FakeHostingEnvironment();
            sessionService = FakeSessionService();
            baseService = FakeBaseService();
            employeeRepository = FakeEmployeeRepository();
            cryptoService = FakeCryptoService();
            var service = new EmployeeManagementService(hostingEnviroment, baseService, cryptoService, sessionService, employeeRepository);

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

        private CreateEmploymentViewModel FakeCreateEmployeeViewModel()
        {
            return fixture.Create<CreateEmploymentViewModel>();
        }

        private IEnumerable<ReferenceResourceModel> FakeListReferenceResourceModel()
        {
            var result = new List<ReferenceResourceModel>();
            var gender = fixture.Build<ReferenceResourceModel>()
                    .With(x => x.RefName, ReferenceResource.Gender)
                    .Create();
            var nationality = fixture.Build<ReferenceResourceModel>()
                    .With(x => x.RefName, ReferenceResource.Nationality)
                    .Create();
            var maritalStatus = fixture.Build<ReferenceResourceModel>()
                    .With(x => x.RefName, ReferenceResource.MaritalStatus)
                    .Create();
            var noOfChildren = fixture.Build<ReferenceResourceModel>()
                    .With(x => x.RefName, ReferenceResource.NoOfChildren)
                    .Create();
            result.Add(gender);
            result.Add(nationality);
            result.Add(maritalStatus);
            result.Add(noOfChildren);

            return result;
        }

        private Fixture InitFixture()
        {
            var fixtureInstance = new Fixture();
            fixtureInstance.Register<IFormFile>(() => null);
            fixtureInstance.Register(() => FakeListReferenceResourceModel());
            fixtureInstance.Register<SelectList>(() => null);

            return fixtureInstance;
        }

        #endregion Helper
    }
}