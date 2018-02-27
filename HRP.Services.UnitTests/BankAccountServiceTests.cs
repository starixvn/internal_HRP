namespace HRP.Services.UnitTests
{
    using Core.Services;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Models.BankAccount;
    using NSubstitute;
    using Xunit;

    public class BankAccountServiceTests
    {
        private readonly IObjectDb mockObjectDb;
        private IBaseService baseService;

        public BankAccountServiceTests()
        {
            DbSettingProviderManager
                .StartTestingSession()
                .Run();

            mockObjectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.Mock((key, time) => mockObjectDb);
        }

        [Fact]
        public void GetBankAccountById_WhenCalled_ReturnAsExpectedBankAccounts()
        {
            // Arrange
            var expected = new BankAccount { BankAccountId = 1, EmployeeId = 1, BankType = 1 };
            var bankAccountService = GetBankAccountService();
            baseService.GetDataEntity<BankAccount>(Arg.Any<ICriteria>()).Returns(expected);

            // Act
            var actual = bankAccountService.GetBankAccountById(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        #region Helpers

        private BankAccountService GetBankAccountService()
        {
            baseService = FakeBaseService();
            return new BankAccountService(baseService);
        }

        private static IBaseService FakeBaseService()
        {
            return Substitute.For<IBaseService>();
        }

        #endregion
    }
}
