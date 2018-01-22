namespace HRP.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.Data;
    using HRP.Core.Constants;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.Employee;
    using HRP.Models;
    using HRP.Models.EmployeeManagement;
    using HRP.Resources;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NSubstitute;
    using Xunit;

    public class EmployeeManagementServiceTests
    {
        private IObjectDb mockObjectDb;
        private Fixture fixture;
        private IBaseService baseService;
        private ISessionService sessionService;
        private ICryptoService cryptoService;
        private EmployeeManagementService employeeService;

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
            employeeService = GetEmployeeManagementService();
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
            employeeService = GetEmployeeManagementService();
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
            employeeService = GetEmployeeManagementService();
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
            employeeService = GetEmployeeManagementService();
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
            employeeService = GetEmployeeManagementService();

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
            employeeService = GetEmployeeManagementService();
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
            employeeService = GetEmployeeManagementService();

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
            employeeService = GetEmployeeManagementService();
            var reader = Substitute.For<IMultiResultReader>();
            reader.Read<CreateEmployeeViewModel>().Returns(new List<CreateEmployeeViewModel>() { expected });
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
            employeeService = GetEmployeeManagementService();
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
            employeeService = GetEmployeeManagementService();

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
            employeeService = GetEmployeeManagementService();

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
            employeeService = GetEmployeeManagementService();

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
            employeeService = GetEmployeeManagementService();

            // Act
            var actual = employeeService.CheckValidPicture(FakePictureProfile());

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(0, actual.Code);
            Assert.Equal(string.Empty, actual.Message);
        }

        #endregion CheckValidPicture

        #region Helper

        private IBaseService FakeBaseService()
        {
            return Substitute.For<IBaseService>();
        }

        private ISessionService FakeSessionService()
        {
            var sessionService = Substitute.For<ISessionService>();
            sessionService.UserId = 99;

            return sessionService;
        }

        private ICryptoService FakeCryptoService()
        {
            return Substitute.For<ICryptoService>();
        }

        private IHostingEnvironment FakeHostingEnvironment()
        {
            return Substitute.For<IHostingEnvironment>();
        }

        private EmployeeManagementService GetEmployeeManagementService()
        {
            var hostingEnviroment = FakeHostingEnvironment();
            sessionService = FakeSessionService();
            baseService = FakeBaseService();
            cryptoService = FakeCryptoService();
            var service = new EmployeeManagementService(hostingEnviroment, baseService, cryptoService, sessionService);

            return service;
        }

        private IFormFile FakePictureProfile(long fileSize = (1024 * 1024), string contentType = "image/png")
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

        private IFormFile FakeTxtFile()
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
            var fixture = new Fixture();
            fixture.Register<IFormFile>(() => null);
            fixture.Register(() => FakeListReferenceResourceModel());
            fixture.Register<SelectList>(() => null);

            return fixture;
        }

        #endregion Helper
    }
}