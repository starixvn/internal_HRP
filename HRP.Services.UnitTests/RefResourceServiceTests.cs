namespace HRP.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Services;
    using Criteria.RefResource;
    using Models;
    using NSubstitute;
    using Xunit;

    public class RefResourceServiceTests
    {
        private IBaseService baseService;

        #region GetReferenceResourcesByName
        [Fact]
        public void GetReferenceResourcesByName_ParamIsNull_ReturnEmptyListReferenceResources()
        {
            // Arrange
            var service = GetRefResourceService();

            // Act
            var actual = service.GetReferenceResourcesByName(null);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public void GetReferenceResourcesByName_ParamIsNotNull_ReturnAsExpectedReferenceResources()
        {
            // Arrange
            var refName = "BankType";
            var expected = FakeRefResources().Where(x => x.RefName == refName);

            var service = GetRefResourceService();
            baseService.GetData<ReferenceResourceModel>(Arg.Any<GetRefResourceByNamesCriteria>()).Returns(expected);

            // Act
            var actual = service.GetReferenceResourcesByName(refName);

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion

        #region GetReferenceResourcesByNames

        [Fact]
        public void GetReferenceResourcesByNames_ParamIsNull_ReturnEmptyListReferenceResources()
        {
            // Arrange
            var service = GetRefResourceService();

            // Act
            var actual = service.GetReferenceResourcesByNames(null);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public void GetReferenceResourcesByNames_ParamIsNotNull_ReturnAsExpectedReferenceResources()
        {
            // Arrange
            var expected = FakeRefResources();

            var refNames = new[]
            {
                "BankType",
                "BankAccountType"
            };

            var service = GetRefResourceService();
            baseService.GetData<ReferenceResourceModel>(Arg.Any<GetRefResourceByNamesCriteria>()).Returns(expected);

            // Act
            var actual = service.GetReferenceResourcesByNames(refNames);

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion GetReferenceResourcesByNames

        #region FilterReferenceResources

        [Fact]
        public void FilterReferenceResources_RefResourcesIsNull_ReturnEmptyListReferenceResources()
        {
            // Arrange
            var service = GetRefResourceService();

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
            var service = GetRefResourceService();

            // Act
            var actual = service.FilterReferenceResources(Arg.Any<IEnumerable<ReferenceResourceModel>>(), refName);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public void FilterReferenceResources_RefResourcesIsNotNullAndRefNameIsValid_ReturnExpectedReferenceResources()
        {
            // Arrange
            var refResources = FakeRefResources();
            var refName = "BankType";
            var service = GetRefResourceService();

            // Act
            var actual = service.FilterReferenceResources(refResources, refName);

            // Assert
            Assert.Single(actual);
            Assert.Equal(refResources.First(), actual.First());
        }



        #endregion FilterReferenceResources

        #region Helpers

        private static List<ReferenceResourceModel> FakeRefResources()
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

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private RefResourceService GetRefResourceService()
        {
            baseService = FakeBaseService();
            var service = new RefResourceService(baseService);

            return service;
        }

        #endregion Helpers
    }
}