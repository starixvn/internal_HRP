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

        ////[Fact]
        ////public void CreateBankAccount_WhenCalled_ReturnCommandResult()
        ////{
        ////    // Arrange
        ////    var expected = new CommandResult();

        ////    var bankAccountService = GetBankAccountService();
        ////    var criteria = new CreateBankAccountCriteria(new BankAccount());

        ////    baseService.ExecuteNonQuery(Arg.Any<CreateBankAccountCriteria>()).Returns(expected);

        ////    // Act
        ////    var actual = bankAccountService.CreateBankAccount(criteria);

        ////    // Assert
        ////    Assert.Equal(expected, actual);
        ////}

        [Fact]
        public void CheckIsAccountNameExisted_InValidEmployeeId_ReturnTrue()
        {
            // Arrange
            var bankAccountService = GetBankAccountService();
            var bankAccount = new BankAccount { EmployeeId = 0 };

            baseService.GetDataEntity<bool>(Arg.Any<ICriteria>()).Returns(false);

            // Act
            var actual = bankAccountService.CheckIsAccountNameExisted(bankAccount);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void CheckIsAccountNameExisted_InValidAccountName_ReturnTrue()
        {
            // Arrange
            var bankAccountService = GetBankAccountService();
            var bankAccount = new BankAccount
            {
                EmployeeId = 1
            };

            baseService.GetDataEntity<bool>(Arg.Any<ICriteria>()).Returns(false);

            // Act
            var actual = bankAccountService.CheckIsAccountNameExisted(bankAccount);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void CheckIsAccountNameExisted_ValidCriteria_ReturnTrue()
        {
            // Arrange
            var bankAccountService = GetBankAccountService();
            var bankAccount = new BankAccount
            {
                EmployeeId = 1,
                AccountName = "Test"
            };

            baseService.GetDataEntity<bool>(Arg.Any<ICriteria>()).Returns(true);

            // Act
            var actual = bankAccountService.CheckIsAccountNameExisted(bankAccount);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void CheckIsAccountNumberExisted_InValidEmployeeId_ReturnTrue()
        {
            // Arrange
            var bankAccountService = GetBankAccountService();
            var bankAccount = new BankAccount { EmployeeId = 0 };

            baseService.GetDataEntity<bool>(Arg.Any<ICriteria>()).Returns(false);

            // Act
            var actual = bankAccountService.CheckIsAccountNumberExisted(bankAccount);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void CheckIsAccountNumberExisted_InValidAccountNumber_ReturnTrue()
        {
            // Arrange
            var bankAccountService = GetBankAccountService();
            var bankAccount = new BankAccount
            {
                EmployeeId = 1
            };

            baseService.GetDataEntity<bool>(Arg.Any<ICriteria>()).Returns(false);

            // Act
            var actual = bankAccountService.CheckIsAccountNumberExisted(bankAccount);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void CheckIsAccountNumberExisted_ValidCriteria_ReturnTrue()
        {
            // Arrange
            var bankAccountService = GetBankAccountService();
            var bankAccount = new BankAccount
            {
                EmployeeId = 1,
                AccountNumber = "Test"
            };

            baseService.GetDataEntity<bool>(Arg.Any<ICriteria>()).Returns(true);

            // Act
            var actual = bankAccountService.CheckIsAccountNumberExisted(bankAccount);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void GetBankAccountById_WhenCalled_ReturnAsExpectedBankAccounts()
        {
            // Arrange
            var expected = new BankAccount { Id = 1, EmployeeId = 1, BankType = 1 };
            var bankAccountService = GetBankAccountService();
            baseService.GetDataEntity<BankAccount>(Arg.Any<ICriteria>()).Returns(expected);

            // Act
            var actual = bankAccountService.GetBankAccountById(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        ////[Fact]
        ////public void UpdateBankAccount_WhenCalled_ReturnAsExpectedCommandResult()
        ////{
        ////    // Arrange
        ////    var bankAccountService = GetBankAccountService();
        ////    var expected = new CommandResult();
        ////    var bankAccount = new BankAccount();
        ////    baseService.ExecuteNonQuery(Arg.Any<ICriteria>()).Returns(expected);

        ////    // Act
        ////    var actual = bankAccountService.UpdateBankAccount(bankAccount);

        ////    // Assert
        ////    Assert.Equal(expected, actual);
        ////}

        ////[Fact]
        ////public void GetBankAccountByEmployeeId_WhenCalled_ReturnAsExpectedModel()
        ////{
        ////    // Arrange
        ////    var bankAccountService = GetBankAccountService();
        ////    var multiResultReader = FakeMultipleResultReader();
        ////    var employeeName = "Test";

        ////    var bankAccounts = new List<BankAccount> { new BankAccount() };

        ////    multiResultReader.Read<BankAccount>().Returns(bankAccounts);
        ////    multiResultReader.ReadSingle<int>().Returns(1);
        ////    multiResultReader.ReadSingle<string>().Returns(employeeName);

        ////    mockObjectDb.QueryMultiple(Arg.Any<ICriteria>()).Returns(multiResultReader);

        ////    // Act
        ////    var actual = bankAccountService.GetBankAccountsByEmployeeId(0);

        ////    // Assert
        ////    Assert.IsType<EmployeeBankAccounts>(actual);
        ////    Assert.Equal(1, actual.CompanyId);
        ////    Assert.Equal(employeeName, actual.EmployeeName);
        ////    Assert.Equal(bankAccounts, actual.BankAccounts);
        ////}

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

            var filteredCount = actual.Count(x => x.IsActive == isActive && bankType == x.BankType);
            var expected = bankAccounts.Count(x => x.IsActive == isActive && bankType == x.BankType);

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

        #region Helpers

        private BankAccountService GetBankAccountService()
        {
            baseService = FakeBaseService();
            return new BankAccountService(baseService);
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

        #endregion
    }
}
