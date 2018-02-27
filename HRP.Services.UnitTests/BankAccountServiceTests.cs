namespace HRP.Services.UnitTests
{
    using System.Collections.Generic;
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
            var nickName = "test";
            var companyId = 1;

            var expected = new BankAccount
            {
                BankAccountId = 1,
                EmployeeId = 1,
                BankType = 1,
                NickName = nickName,
                CompanyId = companyId
            };

            var bankAccountService = GetBankAccountService();
            baseService.GetDataEntity<BankAccount>(Arg.Any<ICriteria>()).Returns(expected);

            // Act
            var actual = bankAccountService.GetBankAccountById(1);

            // Assert
            Assert.Equal(expected, actual);
            Assert.Equal(expected.NickName, nickName);
            Assert.Equal(expected.CompanyId, companyId);
        }

        [Fact]
        public void GetBankAccountByEmployeeId_WhenCalled_ReturnAsExpectedBankAccounts()
        {
            // Arrange
            var expected = new List<BankAccount>
            {
                new BankAccount
                {
                    BankAccountId = 1,
                    EmployeeId = 1,
                    BankType = 1
                }
            };

            var bankAccountService = GetBankAccountService();
            baseService.GetData<BankAccount>(Arg.Any<ICriteria>()).Returns(expected);

            // Act
            var actual = bankAccountService.GetBankAccountsByEmployeeId(1, 1, true);

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
