namespace HRP.Criteria.UnitTests.BankAccount
{
    using System.Collections.Generic;
    using Criteria.BankAccount;
    using Models.BankAccount;
    using Xunit;

    public class UpdateBankAccountCriteriaTest
    {
        ////[Fact]
        ////public void GetSettingKey_ReturnCorrectSettingKey()
        ////{
        ////    // Arrange
        ////    var bankAccount = new BankAccount();
        ////    var criteria = new UpdateBankAccountCriteria(bankAccount);
        ////    var expected = "BankAccount_Update";

        ////    // Act
        ////    var actual = criteria.GetSettingKey();

        ////    // Assert
        ////    Assert.Equal(expected, actual);
        ////}

        ////[Theory]
        ////[MemberData(nameof(IsValidData))]
        ////public void IsValid_WhenCalled_ReturnExpected(UpdateBankAccountCriteria updateBankAccountCriteria, bool expectedResult)
        ////{
        ////    Assert.True(updateBankAccountCriteria.IsValid() == expectedResult);
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

        ////        ////var criteria = new UpdateBankAccountCriteria(bankAccount) { BankName = string.Empty };
        ////        ////data.Add(TheoryDatum.Factory(criteria, false));

        ////        ////criteria = new UpdateBankAccountCriteria(bankAccount) { BankBranch = string.Empty };
        ////        ////data.Add(TheoryDatum.Factory(criteria, false));

        ////        ////criteria = new UpdateBankAccountCriteria(bankAccount) { BankAddress = string.Empty };
        ////        ////data.Add(TheoryDatum.Factory(criteria, false));

        ////        ////criteria = new UpdateBankAccountCriteria(bankAccount) { BankSwiftCode = string.Empty };
        ////        ////data.Add(TheoryDatum.Factory(criteria, false));

        ////        ////criteria = new UpdateBankAccountCriteria(bankAccount) { AccountName = string.Empty };
        ////        ////data.Add(TheoryDatum.Factory(criteria, false));

        ////        ////criteria = new UpdateBankAccountCriteria(bankAccount) { AccountNumber = string.Empty };
        ////        ////data.Add(TheoryDatum.Factory(criteria, false));

        ////        ////criteria = new UpdateBankAccountCriteria(bankAccount) { BeneficiaryName = string.Empty };
        ////        ////data.Add(TheoryDatum.Factory(criteria, false));

        ////        var criteria = new UpdateBankAccountCriteria(bankAccount) { Id = 0 };
        ////        data.Add(TheoryDatum.Factory(criteria, false));

        ////        criteria = new UpdateBankAccountCriteria(bankAccount) { Id = 1 };
        ////        data.Add(TheoryDatum.Factory(criteria, true));

        ////        return data.ConvertAll(d => d.ToParameterArray());
        ////    }
        ////}
    }
}