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
        [MemberData(nameof(IsValidData))]
        public void IsValid_WhenCalled_ReturnExpected(GetBankAccountsByEmployeeIdCriteria getBankAccountsByEmployeeIdCriteria, bool expectedResult)
        {
            Assert.True(getBankAccountsByEmployeeIdCriteria.IsValid() == expectedResult);
        }

        public static IEnumerable<object[]> IsValidData
        {
            get
            {
                var data = new List<TheoryDatum>();

                var criteria = GetBaseCriteria();
                criteria.EmployeeId = 0;
                data.Add(TheoryDatum.Factory(criteria, false));

                data.Add(TheoryDatum.Factory(GetBaseCriteria(), true));

                return data.ConvertAll(d => d.ToParameterArray());
            }
        }

        private static GetBankAccountsByEmployeeIdCriteria GetBaseCriteria()
        {
            return new GetBankAccountsByEmployeeIdCriteria
            {
                EmployeeId = 1
            };
        }
    }
}