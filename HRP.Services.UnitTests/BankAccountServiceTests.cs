namespace HRP.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Services;
    using Fanex.Data.Repository;
    using Models.BankAccount;
    using NSubstitute;
    using Xunit;

    public class BankAccountServiceTests
    {
        private IBaseService baseService;

        [Fact]
        public async Task GetBankAccountById_WhenCalled_ReturnAsExpectedBankAccounts()
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
            baseService.GetDataEntityAsync<BankAccount>(Arg.Any<ICriteria>()).Returns(expected);

            // Act
            var actual = await bankAccountService.GetBankAccountByIdAsync(1);

            // Assert
            Assert.Equal(expected, actual);
            Assert.Equal(expected.NickName, nickName);
            Assert.Equal(expected.CompanyId, companyId);
        }

        [Fact]
        public async Task GetBankAccountByEmployeeId_WhenCalled_ReturnAsExpectedBankAccounts()
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
            baseService.GetDataAsync<BankAccount>(Arg.Any<ICriteria>()).Returns(expected);

            // Act
            var actual = await bankAccountService.GetBankAccountsByEmployeeIdAsync(1, 1, true);

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

        #endregion Helpers
    }
}