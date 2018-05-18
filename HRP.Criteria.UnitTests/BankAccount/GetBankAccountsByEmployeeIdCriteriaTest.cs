namespace HRP.Criteria.UnitTests.BankAccount
{
    using System.Collections.Generic;
    using Criteria.BankAccount;
    using Xunit;

    public class GetBankAccountsByEmployeeIdCriteriaTest
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetBankAccountsByEmployeeIdCriteria
            {
                EmployeeId = 1
            };

            var expected = "BankAccount_GetBankAccountsByEmpId";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(InvalidData))]
        public void IsValid_ParamInvalid_ReturnFalse(GetBankAccountsByEmployeeIdCriteria getBankAccountsByEmployeeIdCriteria)
        {
            Assert.False(getBankAccountsByEmployeeIdCriteria.IsValid());
        }

        [Theory]
        [MemberData(nameof(ValidData))]
        public void IsValid_ParamValid_ReturnTrue(GetBankAccountsByEmployeeIdCriteria getBankAccountsByEmployeeIdCriteria)
        {
            Assert.True(getBankAccountsByEmployeeIdCriteria.IsValid());
        }

        public static IEnumerable<object[]> InvalidData
        {
            get
            {
                var data = new List<object[]>();

                var criteria = GetBaseCriteria();
                criteria.EmployeeId = 0;
                criteria.BankTypeId = null;
                criteria.IsActive = null;

                data.Add(new object[] { criteria });

                return data;
            }
        }

        public static IEnumerable<object[]> ValidData => new List<object[]> { new object[] { GetBaseCriteria() } };

        private static GetBankAccountsByEmployeeIdCriteria GetBaseCriteria()
        {
            return new GetBankAccountsByEmployeeIdCriteria
            {
                EmployeeId = 1
            };
        }
    }
}