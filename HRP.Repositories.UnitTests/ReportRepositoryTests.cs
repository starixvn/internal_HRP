namespace HRP.Repositories.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Models.Report;
    using NSubstitute;
    using Xunit;

    public class ReportRepositoryTests
    {
        private readonly IObjectDb objectDb;

        public ReportRepositoryTests()
        {
            DbSettingProviderManager
                .StartTestingSession()
                .Run();

            objectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.Mock((key, time) => objectDb);
        }

        #region Unit Tests

        [Fact]
        public async Task GetDataForBankInReportAsync_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var multiResultReader = FakeMultipleResultReader();
            var criteria = FakeCriteria();

            multiResultReader.ReadAsync<BankIn>().Returns(new List<BankIn>());
            objectDb.QueryMultipleAsync(Arg.Any<ICriteria>()).Returns(multiResultReader);

            var repository = new ReportRepository();

            // Act
            var actual = await repository.GetDataForBankInReportAsync(criteria);

            // Assert
            Assert.IsType<BankInReport>(actual);
        }

        #endregion

        #region Helpers

        private static IMultiResultReader FakeMultipleResultReader()
        {
            return Substitute.For<IMultiResultReader>();
        }

        private static ICriteria FakeCriteria()
        {
            var criteria = Substitute.For<ICriteria>();
            criteria.GetSettingKey().Returns("HRP_Test");

            return criteria;
        }

        #endregion
    }
}
