namespace HRP.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
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

            baseService.GetDataEntity<bool>(Arg.Any<ICriteria>()).Returns(true);

            // Act
            var actual = bankAccountService.CheckIsBankAccountExisted(bankAccount);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void GetBankAccountById_InValidId_ReturnEmpty()
        {
            // Arrange
            var bankAccountService = GetBankAccountService();
            baseService.GetDataEntity<BankAccount>(Arg.Any<ICriteria>()).Returns(default(BankAccount));

            // Act
            var actual = bankAccountService.GetBankAccountById(0);

            // Assert
            Assert.Equal(default(BankAccount), actual);
        }

        [Fact]
        public void GetBankAccountById_ValidId_ReturnEmpty()
        {
            // Arrange
            var bankAccountService = GetBankAccountService();
            var bankAccount = new BankAccount();
            baseService.GetDataEntity<BankAccount>(Arg.Any<ICriteria>()).Returns(bankAccount);

            // Act
            var actual = bankAccountService.GetBankAccountById(1);

            // Assert
            Assert.Equal(bankAccount, actual);
        }

        [Fact]
        public void UpdateBankAccount_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var bankAccountService = GetBankAccountService();
            var expected = new CommandResult();
            var bankAccount = new BankAccount();
            baseService.ExecuteNonQuery(Arg.Any<ICriteria>()).Returns(expected);

            // Act
            var actual = bankAccountService.UpdateBankAccount(bankAccount);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetBankAccountByEmployeeId_InValidId_ReturnEmpty()
        {
            // Arrange
            var bankAccountService = GetBankAccountService();
            var multiResultReader = FakeMultipleResultReader();

            multiResultReader.Read<BankAccount>().Returns(new List<BankAccount>());
            multiResultReader.ReadSingle<int>().Returns(0);
            multiResultReader.ReadSingle<string>().Returns(string.Empty);

            mockObjectDb.QueryMultiple(Arg.Any<ICriteria>()).Returns(multiResultReader);

            // Act
            var actual = bankAccountService.GetBankAccountsByEmployeeId(0);

            // Assert
            Assert.IsType<EmployeeBankAccounts>(actual);
            Assert.Equal(0, actual.CompanyId);
            Assert.Equal(string.Empty, actual.EmployeeName);
            Assert.Empty(actual.BankAccounts);
        }

        [Fact]
        public void GetBankAccountByEmployeeId_ValidId_ReturnSets()
        {
            // Arrange
            var bankAccountService = GetBankAccountService();
            var multiResultReader = FakeMultipleResultReader();

            var bankAccounts = new List<BankAccount> { new BankAccount() };

            multiResultReader.Read<BankAccount>().Returns(bankAccounts);
            multiResultReader.ReadSingle<int>().Returns(1);
            multiResultReader.ReadSingle<string>().Returns("test");

            mockObjectDb.QueryMultiple(Arg.Any<ICriteria>()).Returns(multiResultReader);

            // Act
            var actual = bankAccountService.GetBankAccountsByEmployeeId(0);

            // Assert
            Assert.IsType<EmployeeBankAccounts>(actual);
            Assert.Equal(1, actual.CompanyId);
            Assert.NotNull(actual.EmployeeName);
            Assert.NotEmpty(actual.BankAccounts);
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(1, false)]
        [InlineData(2, true)]
        [InlineData(2, false)]
        public void FilterBankAccount_FilterDataNotNull_ReturnBankAccounts(int bankType, bool isActive)
        {
            // Arrange
            var bankAccounts = FakeBankAccounts();
            var bankAccountService = GetBankAccountService();
            bool? nullableIsActive = isActive;

            // Act
            var actual = bankAccountService.FilterBankAccounts(bankAccounts, bankType, nullableIsActive);

            var filteredCount = actual.Count(x => x.IsActive = isActive && bankType == x.BankType);
            var expected = bankAccounts.Count(x => x.IsActive = isActive && bankType == x.BankType);

            // Assert
            Assert.Equal(expected, filteredCount);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void FilterBankAccount_IsActiveNull_ReturnBankAccounts(int bankType)
        {
            // Arrange
            var bankAccounts = FakeBankAccounts();
            var bankAccountService = GetBankAccountService();

            // Act
            var actual = bankAccountService.FilterBankAccounts(bankAccounts, bankType, null);

            var filteredCount = actual.Count(x => bankType == x.BankType);
            var expected = bankAccounts.Count(x => bankType == x.BankType);

            // Assert
            Assert.Equal(expected, filteredCount);
        }

        private static IEnumerable<BankAccount> FakeBankAccounts()
        {
            var bankAccounts = new List<BankAccount>
            {
                new BankAccount
                {
                    BankType = 1,
                    IsActive = true
                },
                new BankAccount
                {
                    BankType = 1,
                    IsActive = false
                },
                new BankAccount
                {
                    BankType = 2,
                    IsActive = true
                },
                new BankAccount
                {
                    BankType = 2,
                    IsActive = false
                }
            };

            return bankAccounts;
        }

        private static IBaseService FakeBaseService()
        {
            return Substitute.For<IBaseService>();
        }

        private static IMultiResultReader FakeMultipleResultReader()
        {
            return Substitute.For<IMultiResultReader>();
        }
    }
}
