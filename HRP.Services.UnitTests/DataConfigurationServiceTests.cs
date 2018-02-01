namespace HRP.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.DataConfiguration;
    using HRP.Services.Interfaces;
    using NSubstitute;
    using Xunit;

    public class DataConfigurationServiceTests
    {
        [Fact]
        public async Task GetAllCompanyBVIAsync_DatabaseHasAnyCompany_ReturnListSimpleItem()
        {
            // Arrange
            var fakeBaseService = FakeBaseService();
            var fakeSessionService = FakeSessionService();
            var companies = new List<Company> {
                new Company()
                {
                    CompanyId = 15,
                    Code = "name1",
                    Name = "desc1",
                    IsActive = true
                }
            };

            fakeBaseService.GetDataAsync<Company>(Arg.Any<GetAllCompanyCriteria>()).Returns(companies);
            var dataConfigurationService = GetDataConfigurationService(fakeBaseService, fakeSessionService);

            // Act
            var actual = (await dataConfigurationService.GetAllCompanyBVIAsync()) as List<SimpleItem>;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(companies.Count, actual.Count);
            Assert.Equal(companies[0].CompanyId, actual[0].ItemId);
            Assert.Equal(companies[0].Code, actual[0].ItemName);
            Assert.Equal(companies[0].Name, actual[0].Description);
            Assert.Equal(companies[0].IsActive, actual[0].IsActive);
        }

        private IDataConfigurationService GetDataConfigurationService(IBaseService baseService, ISessionService sessionService)
        {
            var dataConfigurationService = new DataConfigurationService(baseService, sessionService);

            return dataConfigurationService;
        }

        private IBaseService FakeBaseService()
        {
            return Substitute.For<IBaseService>();
        }

        private static ISessionService FakeSessionService()
        {
            var sessionServiceInstance = Substitute.For<ISessionService>();
            sessionServiceInstance.UserId = 99;

            return sessionServiceInstance;
        }
    }
}