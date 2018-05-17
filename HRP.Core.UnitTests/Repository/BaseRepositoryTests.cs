namespace HRP.Core.UnitTests
{
    using System.Collections;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Core.Model;
    using HRP.Core.Repositories;
    using NSubstitute;
    using Xunit;

    public class BaseRepositoryTests
    {
        private readonly IObjectDb objectDb;
        private const string ExceptionMessage = "Object reference not set to an instance of an object.";

        public BaseRepositoryTests()
        {
            DbSettingProviderManager
            .StartTestingSession()
            .Run();

            objectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.Mock((key, time) => objectDb);
        }

        #region ExecuteNonQueryAsync

        [Fact]
        public async Task ExecuteNonQueryAsync_CriteriaNotNull_CallExecuteNonQueryAsync()
        {
            // Arrange
            var criteria = FakeCriteria();
            var repository = new BaseRepository();

            // Action
            await repository.ExecuteNonQueryAsync(criteria);

            // Assert
            await objectDb.Received().ExecuteNonQueryAsync(criteria);
        }

        [Fact]
        public async Task ExecuteNonQueryAsync_OutputParams_ReturnAsResult()
        {
            // Arrange
            var repository = new BaseRepository();
            var criteria = FakeCriteria();
            var outputParams = FakeOutputParamsSuccess();
            var expected = GetSuccessResult();
            objectDb.GetOuputParams().Returns(outputParams);

            // Action
            var result = await repository.ExecuteNonQueryAsync(criteria);

            // Assert
            Assert.Equal(expected.Code, result.Code);
            Assert.Equal(expected.Message, result.Message);
        }

        [Fact]
        public async Task ExecuteNonQueryAsync_CriteriaIsNull_ThrowNullReferenceException()
        {
            // Arrange
            ICriteria criteria = null;
            var repository = new BaseRepository();

            // Act
            var task = Task.Run(() => repository.ExecuteNonQueryAsync(criteria));
            var exception = await Record.ExceptionAsync(async () => await task);

            // Assert
            Assert.Equal(exception.HResult, -2147467261);
            Assert.Equal(exception.Message, ExceptionMessage);
        }

        #endregion ExecuteNonQueryAsync

        #region Helpers

        private static ICriteria FakeCriteria()
        {
            var criteria = Substitute.For<ICriteria>();

            criteria.GetSettingKey().Returns("key");
            criteria.GetSpParams().Returns(new Hashtable());
            criteria.IsValid().Returns(true);

            return criteria;
        }

        private static Hashtable FakeOutputParamsSuccess()
        {
            return new Hashtable
            {
                { "errcode", 0 },
                { "errmsg", string.Empty },
            };
        }

        private CommandResult GetSuccessResult()
        {
            return new CommandResult
            {
                Code = 0,
                Message = string.Empty
            };
        }

        #endregion Helpers
    }
}