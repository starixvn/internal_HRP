namespace HRP.Criteria.UnitTests.BankAccount
{
    using System.Collections.Generic;
    using Criteria.BankAccount;
    using Xunit;

    public class EditBankAccountCriteriaTest
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = GetBaseCriteria();
            var expected = "BankAccount_Update";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(IsValidData))]
        public void IsValid_WhenCalled_ReturnExpected(BankAccountBaseCriteria createBankAccountCriteria, bool expectedResult)
        {
            Assert.True(createBankAccountCriteria.IsValid() == expectedResult);
        }

        private static EditBankAccountCriteria GetBaseCriteria()
        {
            return new EditBankAccountCriteria
            {
                Id = 1,
                EmployeeId = 1,
                BankType = 1,
                BankName = "Test",
                BankBranch = "Test",
                BankAddress = "Test",
                BankSwiftCode = "Test",
                PaymentTransactionId = 1,
                AccountNumber = "Test",
                AccountType = 1,
                BeneficiaryName = "Test",
                IsActive = true,
                UpdatedBy = 1
            };
        }

        public static IEnumerable<object[]> IsValidData
        {
            get
            {
                var data = new List<TheoryDatum>();

                var criteria = GetBaseCriteria();
                criteria.BankName = string.Empty;
                data.Add(TheoryDatum.Factory(criteria, false));

                criteria = GetBaseCriteria();
                criteria.BankBranch = string.Empty;
                data.Add(TheoryDatum.Factory(criteria, true));

                criteria = GetBaseCriteria();
                criteria.BankAddress = string.Empty;
                data.Add(TheoryDatum.Factory(criteria, true));

                criteria = GetBaseCriteria();
                criteria.BankSwiftCode = string.Empty;
                data.Add(TheoryDatum.Factory(criteria, true));

                criteria = GetBaseCriteria();
                criteria.PaymentTransactionId = 1;
                data.Add(TheoryDatum.Factory(criteria, true));

                criteria = GetBaseCriteria();
                criteria.AccountNumber = string.Empty;
                data.Add(TheoryDatum.Factory(criteria, false));

                criteria = GetBaseCriteria();
                criteria.BeneficiaryName = string.Empty;
                data.Add(TheoryDatum.Factory(criteria, false));

                criteria = GetBaseCriteria();
                criteria.Id = 0;
                data.Add(TheoryDatum.Factory(criteria, false));

                criteria = GetBaseCriteria();
                criteria.UpdatedBy = 0;
                data.Add(TheoryDatum.Factory(criteria, false));

                criteria = GetBaseCriteria();
                data.Add(TheoryDatum.Factory(criteria, true));

                return data.ConvertAll(d => d.ToParameterArray());
            }
        }
    }
}