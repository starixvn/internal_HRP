namespace HRP.Criteria.UnitTests.Currency
{
    using HRP.Criteria.Currency;
    using Xunit;

    public class GetCurrencyByIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetCurrencyByIdCriteria();
            var expected = "Currency_GetById";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_JobGradeIdIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetCurrencyByIdCriteria
            {
                CurrencyId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(0)]
        public void IsValid_JobGradeIdIsInvalid_ReturnFalse(int id)
        {
            // Arrange
            var criteria = new GetCurrencyByIdCriteria
            {
                CurrencyId = id
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}