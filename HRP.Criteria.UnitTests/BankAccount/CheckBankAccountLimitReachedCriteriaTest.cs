namespace HRP.Criteria.UnitTests.BankAccount
{
    using System.Collections.Generic;
    using Criteria.BankAccount;
    using Xunit;

    public class CheckBankAccountLimitReachedCriteriaTest
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CheckBankAccountLimitReachedCriteria
            {
                EmployeeId = 1
            };

            var expected = "BankAccount_CheckLimitReached";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(IsValidData))]
        public void IsValid_WhenCalled_ReturnExpected(CheckBankAccountLimitReachedCriteria checkBankAccountLimitReachedCriteria, bool expectedResult)
        {
            Assert.True(checkBankAccountLimitReachedCriteria.IsValid() == expectedResult);
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

        private static CheckBankAccountLimitReachedCriteria GetBaseCriteria()
        {
            return new CheckBankAccountLimitReachedCriteria
            {
                EmployeeId = 1
            };
        }
    }
}