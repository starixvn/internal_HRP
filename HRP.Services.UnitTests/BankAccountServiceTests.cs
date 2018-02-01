namespace HRP.Services.UnitTests
{
    using Core.Model;
    using Core.Services;
    using Criteria.Bank;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Models.Bank;
    using NSubstitute;
    using Xunit;

    public class BankAccountServiceTests
    {
        ////private readonly IObjectDb mockObjectDb;
        private IBaseService baseService;

        public BankAccountServiceTests()
        {
            DbSettingProviderManager
                .StartTestingSession()
                .Run();

            ////mockObjectDb = Substitute.For<IObjectDb>();
            ////ObjectDbFactory.Mock((key, time) => mockObjectDb);
        }

        private BankAccountService GetBankAccountService()
        {
            baseService = FakeBaseService();
            return new BankAccountService(baseService);
        }

        [Fact]
        public void CreateBankAccount_CreateSuccess_ReturnAsExpected()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0,
                Message = "Successful!"
            };

            var bankAccountService = GetBankAccountService();
            var criteria = new CreateBankAccountCriteria(new BankAccount());

            baseService.ExecuteNonQuery(Arg.Any<CreateBankAccountCriteria>()).Returns(expected);

            // Act
            var actual = bankAccountService.CreateBankAccount(criteria);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CheckIsBankAccountExisted_InValidEmployeeId_ReturnTrue()
        {
            // Arrange
            var bankAccountService = GetBankAccountService();
            var bankAccount = new BankAccount { EmployeeId = 0 };

            baseService.GetDataEntity<int>(Arg.Any<ICriteria>()).Returns(0);

            // Act
            var actual = bankAccountService.CheckIsBankAccountExisted(bankAccount);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void CheckIsBankAccountExisted_InValidAccountNameOrAccountNumber_ReturnTrue()
        {
            // Arrange
            var bankAccountService = GetBankAccountService();
            var bankAccount = new BankAccount
            {
                EmployeeId = 1
            };

            baseService.GetDataEntity<int>(Arg.Any<ICriteria>()).Returns(0);

            // Act
            var actual = bankAccountService.CheckIsBankAccountExisted(bankAccount);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void CheckIsBankAccountExisted_ValidCriteria_ReturnTrue()
        {
            // Arrange
            var bankAccountService = GetBankAccountService();
            var bankAccount = new BankAccount
            {
                EmployeeId = 1,
                AccountNumber = "Test",
                AccountName = "Test"
            };

            baseService.GetDataEntity<int>(Arg.Any<ICriteria>()).Returns(1);

            // Act
            var actual = bankAccountService.CheckIsBankAccountExisted(bankAccount);

            // Assert
            Assert.True(actual);
        }

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }
    }
}
