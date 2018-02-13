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
