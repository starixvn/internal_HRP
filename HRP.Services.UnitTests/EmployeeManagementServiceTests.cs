namespace HRP.Services.UnitTests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.Employee;
    using HRP.Models.EmployeeManagement;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using NSubstitute;
    using Xunit;

    public class EmployeeManagementServiceTests
    {
        [Fact]
        public async Task CreateEmployeeAsync_WithValidCreateEmployeeViewModel_ReturnCommandResultSuccessful()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0,
                Message = "Successful!"
            };
            var criteria = new CreateEmployeeCriteria();
            var baseService = FakeBaseService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateEmployeeCriteria>(x => criteria = x)).Returns(expected);

            var viewModel = StubValidCreateEmployeeViewModel();
            var employeeManagementService = InitEmployeeManagementService(baseService);

            // Act
            var actual = await employeeManagementService.CreateEmployeeAsync(viewModel);

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
            var baseService = FakeBaseService();
            baseService.ExecuteNonQueryAsync(Arg.Any<CreateEmployeeCriteria>()).Returns(expected);
            var viewModel = StubValidCreateEmployeeViewModel();
            viewModel.ProfilePicture = StubPictureProfile();
            var employeeManagementService = InitEmployeeManagementService(baseService);

            // Act
            var actual = await employeeManagementService.CreateEmployeeAsync(viewModel);

            // Assert
            Assert.Equal(expected, actual);
        }

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

        private IHostingEnvironment FakeHostingEnvironment()
        {
            return Substitute.For<IHostingEnvironment>();
        }

        private EmployeeManagementService InitEmployeeManagementService(
            IBaseService baseService)
        {
            var hostingEnviroment = FakeHostingEnvironment();
            var sessionService = FakeSessionService();
            var employeeManagementService = new EmployeeManagementService(hostingEnviroment, baseService, null, sessionService, null);

            return employeeManagementService;
        }

        private CreateEmployeeViewModel StubValidCreateEmployeeViewModel()
        {
            return new CreateEmployeeViewModel
            {
                FullName = "Im God",
                NickName = "Zeus",
                Gender = 1,
                DateOfBirth = DateTime.Now,
                PlaceOfBirth = "mexico",
                Nationality = 2,
                NRICNo = "asdasd",
                MaritalStatus = 3,
                NoOfChildren = 4,
                OverseasPhoneNo = "09123456",
                LocalPhoneNo = "091111",
                OtherPhoneNo = "113",
                PassportNo = "12321",
                DateIssue = DateTime.Now,
                ExpiredDate = DateTime.Now.AddYears(20),
                PlaceOfIssue = "Heaven",
                Skype = "mark.zuckerbug",
                PersonalEmail = "ahihi@yahoo.com",
                PermanentAddress = "ahihi",
                ResidenceAddress = "earth",
                MailingAddress = "galaxy",
                EmergencyContact = new EmergencyContact
                {
                    Name = "Donald Trump",
                    Address = "LA",
                    Relationship = "Neighborhood",
                    PhoneNo1 = "114",
                    PhoneNo2 = "115"
                }
            };
        }

        private IFormFile StubPictureProfile()
        {
            var file = Substitute.For<IFormFile>();
            var content = "Hello World from a Fake File";
            var fileName = "test.pdf";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            file.OpenReadStream().Returns(ms);
            file.FileName.Returns(fileName);
            file.Length.Returns(ms.Length);

            return file;
        }
    }
}