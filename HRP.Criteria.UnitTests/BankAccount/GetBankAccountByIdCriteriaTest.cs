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
        [MemberData(nameof(IsValidData))]
        public void IsValid_WhenCalled_ReturnExpected(GetBankAccountByIdCriteria getBankAccountByIdCriteria, bool expectedResult)
        {
            Assert.True(getBankAccountByIdCriteria.IsValid() == expectedResult);
        }

        public static IEnumerable<object[]> IsValidData
        {
            get
            {
                var data = new List<TheoryDatum>();

                var criteria = GetBaseCriteria();
                criteria.Id = 0;
                data.Add(TheoryDatum.Factory(criteria, false));

                data.Add(TheoryDatum.Factory(GetBaseCriteria(), true));

                return data.ConvertAll(d => d.ToParameterArray());
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