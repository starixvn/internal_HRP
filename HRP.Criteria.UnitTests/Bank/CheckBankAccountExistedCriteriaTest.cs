namespace HRP.Criteria.UnitTests.Bank
{
    using System.Collections.Generic;
    using Criteria.Bank;
    using Xunit;

    public class CheckAccountNameExistedCriteriaTest
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CheckAccountNameExistedCriteria
            {
                Id = 1,
                AccountName = "Test",
                EmployeeId = 1
            };

            var expected = "BankAccount_CheckNameExisted";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(IsValidData))]
        public void IsValid_WhenCalled_ReturnExpected(CheckAccountNameExistedCriteria checkAccountNameExistedCriteria, bool expectedResult)
        {
            Assert.True(checkAccountNameExistedCriteria.IsValid() == expectedResult);
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
                criteria.AccountName = string.Empty;
                data.Add(TheoryDatum.Factory(criteria, false));

                criteria = GetBaseCriteria();
                criteria.BankType = 0;
                data.Add(TheoryDatum.Factory(criteria, false));

                data.Add(TheoryDatum.Factory(GetBaseCriteria(), true));

                return data.ConvertAll(d => d.ToParameterArray());
            }
        }

        private static CheckAccountNameExistedCriteria GetBaseCriteria()
        {
            return new CheckAccountNameExistedCriteria
            {
                Id = 1,
                AccountName = "Test",
                EmployeeId = 1,
                BankType = 1
            };
        }
    }
}