namespace HRP.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Services;
    using Fanex.Data.Repository;
    using HRP.Core.Model;
    using HRP.Criteria.BankAccount;
    using Models.BankAccount;
    using NSubstitute;
    using Xunit;

    public class BankAccountServiceTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;

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

        [Fact]
        public async Task CreateBankAccountAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var bankAccountService = GetBankAccountService();
            baseService.ExecuteNonQueryAsync(Arg.Any<CreateBankAccountCriteria>()).Returns(expected);

            // Act
            var actual = await bankAccountService.CreateBankAccountAsync(new CreateBankAccountViewModel());

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task EditBankAccountAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var bankAccountService = GetBankAccountService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditBankAccountCriteria>()).Returns(expected);

            // Act
            var actual = await bankAccountService.EditBankAccountAsync(new EditBankAccountViewModel());

            // Assert
            Assert.Equal(expected, actual);
        }

        #region Helpers

        private BankAccountService GetBankAccountService()
        {
            baseService = FakeBaseService();
            sessionService = FakeSessionService();

            return new BankAccountService(baseService, sessionService);
        }

        private IBaseService FakeBaseService()
        {
            return Substitute.For<IBaseService>();
        }

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId = 1;

            return service;
        }

        #endregion Helpers
    }
}