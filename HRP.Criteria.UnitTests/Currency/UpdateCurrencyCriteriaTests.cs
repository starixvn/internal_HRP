namespace HRP.Criteria.UnitTests.Currency
{
    using HRP.Criteria.Currency;
    using Xunit;

    public class UpdateCurrencyCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UpdateCurrencyCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("Currency_Update", actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new UpdateCurrencyCriteria
            {
                CurrencyId = 1,
                UpdatedBy = 2,
                CurrencyCode = "aa"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
            Assert.IsAssignableFrom<BaseNonQueryCommand>(criteria);
        }

        [Theory]
        [InlineData(-1, "   ")]
        [InlineData(-1, "")]
        [InlineData(-1, "asdasd")]
        [InlineData(0, "asdasd")]
        [InlineData(1, " ")]
        [InlineData(0, " ")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int id, string code)
        {
            // Arrange
            var criteria = new UpdateCurrencyCriteria
            {
                CurrencyId = id,
                UpdatedBy = id,
                CurrencyCode = code
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}