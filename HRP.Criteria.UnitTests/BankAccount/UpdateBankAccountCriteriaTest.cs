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
        [MemberData(nameof(ValidData))]
        public void IsValid_ParamValid_ReturnTrue(BankAccountBaseCriteria createBankAccountCriteria)
        {
            Assert.True(createBankAccountCriteria.IsValid());
        }

        [Theory]
        [MemberData(nameof(InvalidData))]
        public void IsValid_ParamInvalid_ReturnFalse(BankAccountBaseCriteria createBankAccountCriteria)
        {
            Assert.False(createBankAccountCriteria.IsValid());
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

        public static IEnumerable<object[]> InvalidData
        {
            get
            {
                var data = new List<object[]>();

                var criteria = GetBaseCriteria();
                criteria.BankName = string.Empty;
                data.Add(new object[] { criteria });

                criteria = GetBaseCriteria();
                criteria.AccountNumber = string.Empty;
                data.Add(new object[] { criteria });

                criteria = GetBaseCriteria();
                criteria.BeneficiaryName = string.Empty;
                data.Add(new object[] { criteria });

                criteria = GetBaseCriteria();
                criteria.Id = 0;
                data.Add(new object[] { criteria });

                criteria = GetBaseCriteria();
                criteria.UpdatedBy = 0;
                data.Add(new object[] { criteria });

                return data;
            }
        }

        public static IEnumerable<object[]> ValidData
        {
            get
            {
                var data = new List<object[]>();

                var criteria = GetBaseCriteria();
                criteria.BankBranch = string.Empty;
                data.Add(new object[] { criteria });

                criteria = GetBaseCriteria();
                criteria.BankAddress = string.Empty;
                data.Add(new object[] { criteria });

                criteria = GetBaseCriteria();
                criteria.BankSwiftCode = string.Empty;
                data.Add(new object[] { criteria });

                criteria = GetBaseCriteria();
                criteria.PaymentTransactionId = 1;
                data.Add(new object[] { criteria });

                criteria = GetBaseCriteria();
                data.Add(new object[] { criteria });

                return data;
            }
        }
    }
}