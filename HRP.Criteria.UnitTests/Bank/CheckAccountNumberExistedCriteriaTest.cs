namespace HRP.Criteria.UnitTests.Bank
{
    using System.Collections.Generic;
    using Criteria.Bank;
    using Xunit;

    public class CheckAccountNumberExistedCriteriaTest
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CheckAccountNumberExistedCriteria
            {
                Id = 1,
                AccountNumber = "Test",
                EmployeeId = 1,
                BankType = 1
            };

            var expected = "BankAccount_CheckNumberExisted";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(IsValidData))]
        public void IsValid_WhenCalled_ReturnExpected(CheckAccountNumberExistedCriteria checkAccountNumberExistedCriteria, bool expectedResult)
        {
            Assert.True(checkAccountNumberExistedCriteria.IsValid() == expectedResult);
        }

        public static IEnumerable<object[]> IsValidData
        {
            get
            {
                var data = new List<TheoryDatum>();

                var criteria = GetBaseCriteria();
                criteria.EmployeeId = 0;
                data.Add(TheoryDatum.Factory(criteria, false));

                criteria = GetBaseCriteria();
                criteria.BankType = 0;
                data.Add(TheoryDatum.Factory(criteria, false));

                criteria = GetBaseCriteria();
                criteria.AccountNumber = string.Empty;
                data.Add(TheoryDatum.Factory(criteria, false));

                data.Add(TheoryDatum.Factory(GetBaseCriteria(), true));

                return data.ConvertAll(d => d.ToParameterArray());
            }
        }

        private static CheckAccountNumberExistedCriteria GetBaseCriteria()
        {
            return new CheckAccountNumberExistedCriteria
            {
                Id = 1,
                AccountNumber = "Test",
                EmployeeId = 1
            };
        }
    }
}