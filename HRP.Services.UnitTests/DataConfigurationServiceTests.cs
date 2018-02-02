namespace HRP.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.DataConfiguration;
    using HRP.Resources;
    using HRP.Services.Interfaces;
    using NSubstitute;
    using Xunit;

    public class DataConfigurationServiceTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;

        #region CompanyBVI

        [Fact]
        public async Task GetAllCompanyBVIAsync_BaseServiceGetCompanyList_ReturnSimpleItemList()
        {
            // Arrange
            var companies = new List<Company> {
                new Company()
                {
                    CompanyId = 15,
                    Code = "name1",
                    Name = "desc1",
                    IsActive = true
                }
            };

            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<Company>(Arg.Any<GetAllCompanyCriteria>()).Returns(companies);
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

        [Fact]
        public async Task GetAllCompanyBVIAsync_BaseServiceGetEmptyCompanyList_ReturnEmptySimpleItemList()
        {
            // Arrange
            var companies = new List<Company>();

            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<Company>(Arg.Any<GetAllCompanyCriteria>()).Returns(companies);
            // Act
            var actual = (await dataConfigurationService.GetAllCompanyBVIAsync()) as List<SimpleItem>;

            // Assert
            Assert.NotNull(actual);
            Assert.Empty(actual);
            Assert.Equal(companies.Count, actual.Count);
        }

        [Fact]
        public async Task IsExistedCompanyBVIAsync_BaseServiceGetIntegerList_ReturnTrue()
        {
            // Arrange
            var listInteger = new List<int> { 1 };
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<int>(Arg.Any<CheckCompanyExistsCriteria>()).Returns(listInteger);

            // Act
            var actual = await dataConfigurationService.IsExistedCompanyBVIAsync(new SimpleItem());

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public async Task IsExistedCompanyBVIAsync_BaseServiceGetEmptyIntegerList_ReturnFalse()
        {
            // Arrange
            var listInteger = new List<int>();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<int>(Arg.Any<CheckCompanyExistsCriteria>()).Returns(listInteger);

            // Act
            var actual = await dataConfigurationService.IsExistedCompanyBVIAsync(new SimpleItem());

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public async Task CreateCompanyBVIAsync_BaseServiceExecuteNonQuerySuccessful_ReturnCommandResultSuccessfulWithSuccessfulMessage()
        {
            // Arrange
            var criteria = new CreateCompanyCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateCompanyCriteria>(x => criteria = x)).Returns(new CommandResult { Code = 0 });

            // Act
            var actual = await dataConfigurationService.CreateCompanyBVIAsync(MockSimpleItem());

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.True(actual.IsSuccessful);
            Assert.Equal(string.Format(Message.ItemCreatedSuccessful, criteria.Code), actual.Message);
        }

        [Fact]
        public async Task CreateCompanyBVIAsync_BaseServiceExecuteNonQueryNotSuccessful_ReturnCommandResultNotSuccessful()
        {
            // Arrange
            var criteria = new CreateCompanyCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateCompanyCriteria>(x => criteria = x)).Returns(new CommandResult());

            // Act
            var actual = await dataConfigurationService.CreateCompanyBVIAsync(new SimpleItem());

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.False(actual.IsSuccessful);
            Assert.False(criteria.IsValid());
        }

        [Fact]
        public async Task GetCompanyBVIByIdAsync_BaseServiceGetCompanyInstance_ReturnSimpleItemInstance()
        {
            // Arrange
            var company = new Company
            {
                CompanyId = 11,
                Code = "name",
                Name = "desc",
                IsActive = true
            };

            var dataConfigurationService = GetDataConfigurationService();

            baseService.GetDataEntityAsync<Company>(Arg.Any<GetCompanyByIdCriteria>()).Returns(company);

            // Act
            var actual = await dataConfigurationService.GetCompanyBVIByIdAsync(11);

            // Assert
            Assert.IsType<SimpleItem>(actual);
            Assert.Equal(company.Code, actual.ItemName);
            Assert.Equal(company.Name, actual.Description);
            Assert.Equal(company.IsActive, actual.IsActive);
            Assert.Equal(company.CompanyId, actual.ItemId);
        }

        [Fact]
        public async Task GetCompanyBVIByIdAsync_BaseServiceGetNullCompany_ReturnNull()
        {
            // Arrange
            Company company = null;
            var dataConfigurationService = GetDataConfigurationService();

            baseService.GetDataEntityAsync<Company>(Arg.Any<GetCompanyByIdCriteria>()).Returns(company);

            // Act
            var actual = await dataConfigurationService.GetCompanyBVIByIdAsync(11);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task UpdateCompanyBVIAsync_BaseServiceExecuteNonQuerySuccessful_ReturnCommandResultSuccessfulWithSuccessfulMessage()
        {
            // Arrange
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Any<UpdateCompanyCriteria>()).Returns(new CommandResult { Code = 0 });

            // Act
            var actual = await dataConfigurationService.UpdateCompanyBVIAsync(MockSimpleItem());

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.True(actual.IsSuccessful);
            Assert.Equal(Message.ItemUpdatedSuccessful, actual.Message);
        }

        #endregion CompanyBVI

        #region Division

        [Fact]
        public async void GetAllDivisionAsync_BaseServiceGetDivisionList_ReturnDivisionList()
        {
            // Arrange
            var expected = new List<Division>
            {
                new Division
                {
                    DivisionId = 9,
                    Code = "div",
                    Name = "desc",
                    IsActive= false,
                    CompanyId = 1,
                    CompanyCode = "OW"
                }
            };
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<Division>(Arg.Any<GetAllDivisionCriteria>()).Returns(expected);

            // Act
            var actual = await dataConfigurationService.GetAllDivisionAsync() as List<Division>;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expected.Count, actual.Count);
        }

        #endregion Division

        private IDataConfigurationService GetDataConfigurationService()
        {
            baseService = FakeBaseService();
            sessionService = FakeSessionService();
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

        private SimpleItem MockSimpleItem()
        {
            return new SimpleItem
            {
                ItemId = 22,
                ItemName = "name1",
                Description = "desc1",
                IsActive = true
            };
        }
    }
}