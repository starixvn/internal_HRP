namespace HRP.Criteria.UnitTests.BankAccount
{
    using System.Collections.Generic;
    using Criteria.BankAccount;
    using Xunit;

    public class CheckLimitReachedCriteriaTest
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CheckLimitReachedCriteria
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
        public void IsValid_WhenCalled_ReturnExpected(CheckLimitReachedCriteria checkLimitReachedCriteria, bool expectedResult)
        {
            Assert.True(checkLimitReachedCriteria.IsValid() == expectedResult);
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

        private static CheckLimitReachedCriteria GetBaseCriteria()
        {
            return new CheckLimitReachedCriteria
            {
                EmployeeId = 1
            };
        }
    }
}