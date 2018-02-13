namespace HRP.Criteria.UnitTests.Bank
{
    using System.Collections.Generic;
    using Criteria.Bank;
    using Models.Bank;
    using Xunit;

    public class CreateBankAccountCriteriaTest
    {
        ////[Fact]
        ////public void GetSettingKey_ReturnCorrectSettingKey()
        ////{
        ////    // Arrange
        ////    var bankAccount = new BankAccount();
        ////    var criteria = new CreateBankAccountCriteria(bankAccount);
        ////    var expected = "BankAccount_CreateBankAccount";

        ////    // Act
        ////    var actual = criteria.GetSettingKey();

        ////    // Assert
        ////    Assert.Equal(expected, actual);
        ////}

        ////[Theory]
        ////[MemberData(nameof(IsValidData))]
        ////public void IsValid_WhenCalled_ReturnExpected(CreateBankAccountCriteria createBankAccountCriteria, bool expectedResult)
        ////{
        ////    Assert.True(createBankAccountCriteria.IsValid() == expectedResult);
        ////}

        ////public static IEnumerable<object[]> IsValidData
        ////{
        ////    get
        ////    {
        ////        var bankAccount = new BankAccount
        ////        {
        ////            Id = 1,
        ////            EmployeeId = 1,
        ////            BankType = 1,
        ////            BankName = "Test",
        ////            BankBranch = "Test",
        ////            BankAddress = "Test",
        ////            BankSwiftCode = "Test",
        ////            AccountName = "Test",
        ////            AccountNumber = "Test",
        ////            AccountType = 1,
        ////            BeneficiaryName = "Test",
        ////            IsActive = true
        ////        };

        ////        var data = new List<TheoryDatum>();

        ////        var criteria = new CreateBankAccountCriteria(bankAccount) { BankName = string.Empty };
        ////        data.Add(TheoryDatum.Factory(criteria, false));

        ////        criteria = new CreateBankAccountCriteria(bankAccount) { BankBranch = string.Empty };
        ////        data.Add(TheoryDatum.Factory(criteria, false));

        ////        criteria = new CreateBankAccountCriteria(bankAccount) { BankAddress = string.Empty };
        ////        data.Add(TheoryDatum.Factory(criteria, false));

        ////        criteria = new CreateBankAccountCriteria(bankAccount) { BankSwiftCode = string.Empty };
        ////        data.Add(TheoryDatum.Factory(criteria, true));

        ////        criteria = new CreateBankAccountCriteria(bankAccount) { AccountName = string.Empty };
        ////        data.Add(TheoryDatum.Factory(criteria, false));

        ////        criteria = new CreateBankAccountCriteria(bankAccount) { AccountNumber = string.Empty };
        ////        data.Add(TheoryDatum.Factory(criteria, false));

        ////        criteria = new CreateBankAccountCriteria(bankAccount) { BeneficiaryName = string.Empty };
        ////        data.Add(TheoryDatum.Factory(criteria, false));

        ////        criteria = new CreateBankAccountCriteria(bankAccount) { Id = 0 };
        ////        data.Add(TheoryDatum.Factory(criteria, true));

        ////        criteria = new CreateBankAccountCriteria(bankAccount);
        ////        data.Add(TheoryDatum.Factory(criteria, true));

        ////        return data.ConvertAll(d => d.ToParameterArray());
        ////    }
        ////}
    }
}
