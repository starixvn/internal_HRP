namespace HRP.Core.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRP.Core.Services;
    using Model;
    using NSubstitute;
    using Repositories;
    using Xunit;

    public class BaseServiceTests
    {
        private IDynamicRepository dynamicRepository;
        private IBaseRepository baseRepository;

        #region GetDataAsync

        [Fact]
        public async Task GetDataAsync_CriteriaIsNull_ReturnEmptyList()
        {
            // Arrange
            var baseService = GetBaseService();

            // Act
            var actual = await baseService.GetDataAsync<int>(null);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetDataAsync_CriteriaIsInvalid_ReturnEmptyList()
        {
            // Arrange
            var criteria = Substitute.For<ICriteria>();
            criteria.IsValid().Returns(false);
            var baseService = GetBaseService();

            // Act
            var actual = await baseService.GetDataAsync<FunctionModel>(criteria);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetDataAsync_CriteriaIsValid_ReturnExpectedData()
        {
            // Arrange
            var expected = new List<FunctionModel> {
                new FunctionModel { FunctionId = 1, FunctionKey = "Configuration", Name = "Configuration", CompanyId = 1, ParentId = null, IsReadOnly = false, IsRead = true, IsHost = true }
            };
            var criteria = Substitute.For<ICriteria>();
            criteria.IsValid().Returns(true);
            var baseService = GetBaseService();
            dynamicRepository.FetchAsync<FunctionModel>(criteria).Returns(expected);

            // Act
            var actual = await baseService.GetDataAsync<FunctionModel>(criteria);

            // Assert
            Assert.NotEmpty(actual);
            Assert.Equal(expected, actual);
        }

        #endregion GetDataAsync

        #region GetDataEntityAsync

        [Fact]
        public async Task GetDataEntityAsync_CriteriaIsNull_ReturnDefaultValue()
        {
            // Arrange
            var baseService = GetBaseService();

            // Act
            var actual = await baseService.GetDataEntityAsync<int>(null);

            // Assert
            Assert.Equal(0, actual);
        }

        [Fact]
        public async Task GetDataEntityAsync_CriteriaIsInvalid_ReturnDefaultValue()
        {
            // Arrange
            var criteria = Substitute.For<ICriteria>();
            criteria.IsValid().Returns(false);
            var baseService = GetBaseService();

            // Act
            var actual = await baseService.GetDataEntityAsync<int>(criteria);

            // Assert
            Assert.Equal(0, actual);
        }

        [Fact]
        public async Task GetDataEntityAsync_CriteriaIsValid_ReturnExpectedData()
        {
            // Arrange
            var expected = 1;
            var criteria = Substitute.For<ICriteria>();
            criteria.IsValid().Returns(true);
            var baseService = GetBaseService();
            dynamicRepository.GetAsync<int>(criteria).Returns(expected);

            // Act
            var actual = await baseService.GetDataEntityAsync<int>(criteria);

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion GetDataEntityAsync

        #region ExecuteNonQueryAsync

        [Fact]
        public async Task ExecuteNonQueryAsync_CriteriaIsNull_ReturnDefaultCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var baseService = GetBaseService();

            // Act
            var actual = await baseService.ExecuteNonQueryAsync(null);

            // Assert
            Assert.Equal(expected.Code, actual.Code);
            Assert.Equal(expected.IsSuccessful, actual.IsSuccessful);
            Assert.Equal(expected.Message, actual.Message);
        }

        [Fact]
        public async Task ExecuteNonQueryAsync_CriteriaIsInvalid_ReturnDefaultCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var criteria = Substitute.For<ICriteria>();
            criteria.IsValid().Returns(false);
            var baseService = GetBaseService();

            // Act
            var actual = await baseService.ExecuteNonQueryAsync(criteria);

            // Assert
            Assert.Equal(expected.Code, actual.Code);
            Assert.Equal(expected.IsSuccessful, actual.IsSuccessful);
            Assert.Equal(expected.Message, actual.Message);
        }

        [Fact]
        public async Task ExecuteNonQueryAsync_CriteriaIsValid_ExpectedExecuteQuery()
        {
            // Arrange
            var criteria = Substitute.For<ICriteria>();
            criteria.IsValid().Returns(true);
            var baseService = GetBaseService();

            // Act
            await baseService.ExecuteNonQueryAsync(criteria);

            // Assert
            await baseRepository.Received().ExecuteNonQueryAsync(Arg.Is<ICriteria>(x => x.IsValid()));
        }

        #endregion ExecuteNonQueryAsync

        #region Helpers

        private static IDynamicRepository FakeDynamicRepository()
        {
            var repository = Substitute.For<IDynamicRepository>();

            return repository;
        }

        private static IBaseRepository FakeBaseRepository()
        {
            var repository = Substitute.For<IBaseRepository>();

            return repository;
        }

        private BaseService GetBaseService()
        {
            dynamicRepository = FakeDynamicRepository();
            baseRepository = FakeBaseRepository();
            var service = new BaseService(dynamicRepository, baseRepository);

            return service;
        }

        #endregion Helpers
    }
}