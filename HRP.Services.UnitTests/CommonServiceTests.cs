namespace HRP.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Services;
    using HRP.Core.Criteria;
    using HRP.Core.Model;
    using HRP.Criteria;
    using HRP.Models;
    using NSubstitute;
    using Xunit;

    public class CommonServiceTests
    {
        private IBaseService baseService;

        #region GetReferenceResourcesByNames

        [Fact]
        public async Task GetReferenceResourcesByNames_ParamIsNull_ReturnEmptyListReferenceResources()
        {
            // Arrange
            var service = GetCommonService();

            // Act
            var actual = await service.GetReferenceResourcesByNamesAsync(null);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetReferenceResourcesByNames_ParamIsNotNull_ReturnAsExpectedReferenceResources()
        {
            // Arrange
            var expected = GetRefResourcesForTest();

            var refNames = new[]
            {
                "BankType",
                "BankAccountType"
            };

            var service = GetCommonService();
            baseService.GetDataAsync<ReferenceResourceModel>(Arg.Any<GetRefResourceByNamesCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetReferenceResourcesByNamesAsync(refNames);

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion GetReferenceResourcesByNames

        #region FilterReferenceResources

        [Fact]
        public void FilterReferenceResources_RefResourcesIsNull_ReturnEmptyListReferenceResources()
        {
            // Arrange
            var service = GetCommonService();

            // Act
            var actual = service.FilterReferenceResources(null, null);

            // Assert
            Assert.Empty(actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FilterReferenceResources_RefNameIsInvalid_ReturnEmptyListReferenceResources(string refName)
        {
            // Arrange
            var service = GetCommonService();

            // Act
            var actual = service.FilterReferenceResources(Arg.Any<IEnumerable<ReferenceResourceModel>>(), refName);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public void FilterReferenceResources_RefResourcesIsNotNullAndRefNameIsValid_ReturnExpectedReferenceResources()
        {
            // Arrange
            var refResources = GetRefResourcesForTest();
            var refName = "BankType";
            var service = GetCommonService();

            // Act
            var actual = service.FilterReferenceResources(refResources, refName);

            // Assert
            Assert.Single(actual);
            Assert.Equal(refResources.First(), actual.First());
        }

        #endregion FilterReferenceResources

        [Fact]
        public async Task GetCompanyByFunctionKeyAsync_WhenCalled_ReturnAsExpectedCompanies()
        {
            // Arrange
            var companies = new List<Company>
            {
                new Company{ CompanyId = 1 }
            };
            var service = GetCommonService();
            baseService.GetDataAsync<Company>(Arg.Any<GetCompanyByFunctionKeyCriteria>()).Returns(companies);

            // Act
            var actual = await service.GetCompanyByFunctionKeyAsync("FunctionKey");

            // Assert
            Assert.Equal(companies, actual);
        }

        [Fact]
        public void GetSelectList_WhenCalled_ReturnAsExpectedSelectList()
        {
            // Arrange
            var refResources = GetRefResourcesForTest();
            var service = GetCommonService();

            // Act
            var actual = service.GetSelectList(refResources);

            // Assert
            Assert.Equal(refResources.Count, actual.Count());
        }

        #region Helpers

        private List<ReferenceResourceModel> GetRefResourcesForTest()
        {
            return new List<ReferenceResourceModel>
            {
                new ReferenceResourceModel
                {
                    ResourceId = 1,
                    RefName = "BankType",
                    Value = "1",
                    Description = "DAR",
                    IsActive = true
                },
                new ReferenceResourceModel
                {
                    ResourceId = 2,
                    RefName = "LocalCompany",
                    Value = "1",
                    Description = "NEX",
                    IsActive = true
                }
            };
        }

        private IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId = 1;

            return service;
        }

        private CommonService GetCommonService()
        {
            baseService = FakeBaseService();
            var sessionService = FakeSessionService();
            var service = new CommonService(baseService, sessionService);

            return service;
        }

        #endregion Helpers
    }
}