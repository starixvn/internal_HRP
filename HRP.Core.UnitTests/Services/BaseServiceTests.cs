namespace HRP.Core.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRP.Core.Model;
    using HRP.Core.Repositories;
    using HRP.Core.Services;
    using NSubstitute;
    using Xunit;

    public class BaseServiceTests
    {
        private IDynamicRepository dynamicRepository;
        private IBaseRepository baseRepository;
        private BaseService baseService;

        #region GetData

        [Fact]
        public void GetData_CriteriaIsNull_ReturnEmptyList()
        {
            // Arrange
            baseService = GetBaseService();

            // Act
            var actual = baseService.GetData<int>(null);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public void GetData_CriteriaIsInvalid_ReturnEmptyList()
        {
            // Arrange
            var criteria = Substitute.For<ICriteria>();
            criteria.IsValid().Returns(false);
            baseService = GetBaseService();

            // Act
            var actual = baseService.GetData<int>(criteria);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public void GetData_CriteriaIsValid_ReturnExpectedData()
        {
            // Arrange
            var expected = new List<int> { 1, 2, 3 };
            var criteria = Substitute.For<ICriteria>();
            criteria.IsValid().Returns(true);
            baseService = GetBaseService();
            dynamicRepository.Fetch<int>(criteria).Returns(expected);

            // Act
            var actual = baseService.GetData<int>(criteria);

            // Assert
            Assert.NotEmpty(actual);
            Assert.Equal(expected, actual);
        }

        #endregion GetData

        #region GetDataAsync

        [Fact]
        public async Task GetDataAsync_CriteriaIsNull_ReturnEmptyList()
        {
            // Arrange
            baseService = GetBaseService();

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
            baseService = GetBaseService();

            // Act
            var actual = await baseService.GetDataAsync<int>(criteria);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetDataAsync_CriteriaIsValid_ReturnExpectedData()
        {
            // Arrange
            var expected = new List<int> { 1, 2, 3 };
            var criteria = Substitute.For<ICriteria>();
            criteria.IsValid().Returns(true);
            baseService = GetBaseService();
            dynamicRepository.FetchAsync<int>(criteria).Returns(expected);

            // Act
            var actual = await baseService.GetDataAsync<int>(criteria);

            // Assert
            Assert.NotEmpty(actual);
            Assert.Equal(expected, actual);
        }

        #endregion GetDataAsync

        #region GetDataEntity

        [Fact]
        public void GetDataEntity_CriteriaIsNull_ReturnDefaultValue()
        {
            // Arrange
            baseService = GetBaseService();

            // Act
            var actual = baseService.GetDataEntity<int>(null);

            // Assert
            Assert.Equal(0, actual);
        }

        [Fact]
        public void GetDataEntity_CriteriaIsInvalid_ReturnDefaultValue()
        {
            // Arrange
            var criteria = Substitute.For<ICriteria>();
            criteria.IsValid().Returns(false);
            baseService = GetBaseService();

            // Act
            var actual = baseService.GetDataEntity<int>(criteria);

            // Assert
            Assert.Equal(0, actual);
        }

        [Fact]
        public void GetDataEntity_CriteriaIsValid_ReturnExpectedData()
        {
            // Arrange
            var expected = 1;
            var criteria = Substitute.For<ICriteria>();
            criteria.IsValid().Returns(true);
            baseService = GetBaseService();
            dynamicRepository.Get<int>(criteria).Returns(expected);

            // Act
            var actual = baseService.GetDataEntity<int>(criteria);

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion GetDataEntity

        #region GetDataEntityAsync

        [Fact]
        public async Task GetDataEntityAsync_CriteriaIsNull_ReturnDefaultValue()
        {
            // Arrange
            baseService = GetBaseService();

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
            baseService = GetBaseService();

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
            baseService = GetBaseService();
            dynamicRepository.GetAsync<int>(criteria).Returns(expected);

            // Act
            var actual = await baseService.GetDataEntityAsync<int>(criteria);

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion GetDataEntityAsync

        #region ExecuteNonQuery

        [Fact]
        public void ExecuteNonQuery_CriteriaIsNull_ReturnDefaultCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            baseService = GetBaseService();

            // Act
            var actual = baseService.ExecuteNonQuery(null);

            // Assert
            Assert.Equal(expected.Code, actual.Code);
            Assert.Equal(expected.IsSuccessful, actual.IsSuccessful);
            Assert.Equal(expected.Message, actual.Message);
        }

        [Fact]
        public void ExecuteNonQuery_CriteriaIsInvalid_ReturnDefaultCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var criteria = Substitute.For<ICriteria>();
            criteria.IsValid().Returns(false);
            baseService = GetBaseService();

            // Act
            var actual = baseService.ExecuteNonQuery(criteria);

            // Assert
            Assert.Equal(expected.Code, actual.Code);
            Assert.Equal(expected.IsSuccessful, actual.IsSuccessful);
            Assert.Equal(expected.Message, actual.Message);
        }

        [Fact]
        public void ExecuteNonQuery_CriteriaIsValid_ExpectedExecuteQuery()
        {
            // Arrange
            var criteria = Substitute.For<ICriteria>();
            criteria.IsValid().Returns(true);
            baseService = GetBaseService();

            // Act
            baseService.ExecuteNonQuery(criteria);

            // Assert
            baseRepository.Received().ExecuteNonQuery(Arg.Is<ICriteria>(x => x.IsValid()));
        }

        #endregion ExecuteNonQuery

        #region ExecuteNonQueryAsync

        [Fact]
        public async Task ExecuteNonQueryAsync_CriteriaIsNull_ReturnDefaultCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            baseService = GetBaseService();

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
            baseService = GetBaseService();

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
            baseService = GetBaseService();

            // Act
            await baseService.ExecuteNonQueryAsync(criteria);

            // Assert
            await baseRepository.Received().ExecuteNonQueryAsync(Arg.Is<ICriteria>(x => x.IsValid()));
        }

        #endregion ExecuteNonQueryAsync

        #region Helpers

        private IDynamicRepository FakeDynamicRepository()
        {
            var repository = Substitute.For<IDynamicRepository>();

            return repository;
        }

        private IBaseRepository FakeBaseRepository()
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