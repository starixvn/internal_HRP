namespace HRP.Services.UnitTests
{
    using System;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.Employee;
    using HRP.Models.EmployeeManagement;
    using HRP.Repositories.Interfaces;
    using Microsoft.AspNetCore.Hosting;
    using NSubstitute;
    using Xunit;

    public class EmployeeManagementServiceTests
    {
        [Fact]
        public async Task CreateEmployeeAsync_WhenCall_ReturnCommandResult()
        {
            // Arange
            var expected = new CommandResult();
            var baseService = FakeBaseService();
            baseService.ExecuteNonQueryAsync(Arg.Any<CreateEmployeeCriteria>()).Returns(expected);
            var hostingEnviroment = FakeHostingEnvironment();
            var sessionService = FakeSessionService();
            var employeeManagementService = InitEmployeeManagementService(hostingEnviroment, baseService, null, sessionService, null);

            // Act
            var actual = await employeeManagementService.CreateEmployeeAsync(FakeCreateEmployeeViewModel());

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
            sessionService.UserId = 1;

            return sessionService;
        }

        private IHostingEnvironment FakeHostingEnvironment()
        {
            return Substitute.For<IHostingEnvironment>();
        }

        private EmployeeManagementService InitEmployeeManagementService(
            IHostingEnvironment hostingEnvironment,
            IBaseService baseService,
            ICertificateService certificateService,
            ISessionService sessionService,
            IEmployeeManagementRepository employeeRepository)
        {
            var employeeManagementService = new EmployeeManagementService(hostingEnvironment, baseService, certificateService, sessionService, employeeRepository);
            return employeeManagementService;
        }

        private CreateEmployeeViewModel FakeCreateEmployeeViewModel()
        {
            return new CreateEmployeeViewModel
            {
                FullName = "fn",
                NickName = "nn",
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
                PermanentAddress = "ahihi",
                ResidenceAddress = "earth",
                EmergencyContact = new EmergencyContact
                {
                    Name = "Donald Trump",
                    Address = "LA",
                    Relationship = "Neighborhood",
                    PhoneNo1 = "114"
                }
            };
        }
    }
}