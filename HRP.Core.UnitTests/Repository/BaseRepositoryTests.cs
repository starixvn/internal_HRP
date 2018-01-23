namespace HRP.Core.UnitTests.Repository
{
    using System;
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
        private readonly BaseRepositoryTestable baseRepository;
        private readonly IObjectDb objectDb;
        private const string ExceptionMessage = "Object reference not set to an instance of an object.";

        public BaseRepositoryTests()
        {
            objectDb = Substitute.For<IObjectDb>();

            baseRepository = Substitute.ForPartsOf<BaseRepositoryTestable>();
            baseRepository.When(r => r.Public_GetObjectDb(Arg.Any<string>())).DoNotCallBase();
            baseRepository.Public_GetObjectDb(Arg.Any<string>()).Returns(objectDb);
        }

        [Fact]
        public void ExecuteNonQuery_CriteriaIsNull_ThrowNullReferenceException()
        {
            // Arrange
            ICriteria criteria = null;

            // Action
            Action testDelegate = () => baseRepository.Public_ExecuteNonQuery(criteria);

            // Assert
            Assert.Throws<NullReferenceException>(testDelegate);
        }

        [Fact]
        public void ExecuteNonQuery_CriteriaNotNull_CallGetObjectDb()
        {
            // Arrange
            var criteria = FakeCriteria();
            var expected = criteria.GetSettingKey();

            // Action
            baseRepository.Public_ExecuteNonQuery(criteria);

            // Assert
            baseRepository.Received().Public_GetObjectDb(expected);
        }

        [Fact]
        public void ExecuteNonQuery_CriteriaNotNull_CallExecuteNonQuery()
        {
            // Arrange
            var criteria = FakeCriteria();

            // Action
            baseRepository.Public_ExecuteNonQuery(criteria);

            // Assert
            objectDb.Received().ExecuteNonQuery(criteria);
        }

        [Fact]
        public void ExecuteNonQuery_OutputParams_ReturnAsResult()
        {
            // Arrange
            var criteria = FakeCriteria();
            var outputParams = FakeOutputParamsSuccess();
            var expected = GetSuccessResult();
            objectDb.GetOuputParams().Returns(outputParams);

            // Action
            var result = baseRepository.Public_ExecuteNonQuery(criteria);

            // Assert
            Assert.Equal(expected.Code, result.Code);
            Assert.Equal(expected.Message, result.Message);
        }

        [Fact]
        public void ExecuteNonQuery_AfterUseObjectDb_Dispose()
        {
            // Arrange
            var criteria = FakeCriteria();

            // Action
            baseRepository.Public_ExecuteNonQuery(criteria);

            // Assert
            objectDb.Received().Dispose();
        }

        [Fact]
        public async Task ExecuteNonQueryAsync_CriteriaIsNull_ThrowNullReferenceException()
        {
            // Arrange
            ICriteria criteria = null;

            var task = Task.Run(() => baseRepository.Public_ExecuteNonQueryAsync(criteria));
            var exception = await Record.ExceptionAsync(async () => await task);

            Assert.Equal(exception.HResult, -2147467261);
            Assert.Equal(exception.Message, ExceptionMessage);
        }

        [Fact]
        public async Task ExecuteNonQueryAsync_CriteriaNotNull_CallGetObjectDb()
        {
            // Arrange
            var criteria = FakeCriteria();
            var expected = criteria.GetSettingKey();

            // Action
            await baseRepository.Public_ExecuteNonQueryAsync(criteria);

            // Assert
            baseRepository.Received().Public_GetObjectDb(expected);
        }

        [Fact]
        public async Task ExecuteNonQueryAsync_CriteriaNotNull_CallExecuteNonQueryAsync()
        {
            // Arrange
            var criteria = FakeCriteria();

            // Action
            await baseRepository.Public_ExecuteNonQueryAsync(criteria);

            // Assert
            await objectDb.Received().ExecuteNonQueryAsync(criteria);
        }

        [Fact]
        public async Task ExecuteNonQueryAsync_OutputParams_ReturnAsResult()
        {
            // Arrange
            var criteria = FakeCriteria();
            var outputParams = FakeOutputParamsSuccess();
            var expected = GetSuccessResult();
            objectDb.GetOuputParams().Returns(outputParams);

            // Action
            var result = await baseRepository.Public_ExecuteNonQueryAsync(criteria);

            // Assert
            Assert.Equal(expected.Code, result.Code);
            Assert.Equal(expected.Message, result.Message);
        }

        [Fact]
        public async Task ExecuteNonQueryAsync_AfterUseObjectDb_Dispose()
        {
            // Arrange
            var criteria = FakeCriteria();

            // Action
            await baseRepository.Public_ExecuteNonQueryAsync(criteria);

            // Assert
            objectDb.Received().Dispose();
        }

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
    }
}