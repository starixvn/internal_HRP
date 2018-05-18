namespace HRP.Criteria.UnitTests.BankAccount
{
    using System.Collections.Generic;
    using Criteria.BankAccount;
    using Xunit;

    public class GetBankAccountByIdCriteriaTest
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetBankAccountByIdCriteria
            {
                Id = 1
            };

            var expected = "BankAccount_GetBankAccountById";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(ValidData))]
        public void IsValid_ParamValid_ReturnTrue(GetBankAccountByIdCriteria getBankAccountByIdCriteria)
        {
            Assert.True(getBankAccountByIdCriteria.IsValid());
        }

        [Theory]
        [MemberData(nameof(InvalidData))]
        public void IsValid_ParamInValid_ReturnFalse(GetBankAccountByIdCriteria getBankAccountByIdCriteria)
        {
            Assert.False(getBankAccountByIdCriteria.IsValid());
        }

        public static IEnumerable<object[]> ValidData => new List<object[]> { new object[] { GetBaseCriteria() } };

        public static IEnumerable<object[]> InvalidData
        {
            get
            {
                var data = new List<object[]>();

                var criteria = GetBaseCriteria();
                criteria.Id = 0;
                data.Add(new object[]{criteria});

                return data;
            }
        }

        private static GetBankAccountByIdCriteria GetBaseCriteria()
        {
            return new GetBankAccountByIdCriteria
            {
                Id = 1
            };
        }
    }
}