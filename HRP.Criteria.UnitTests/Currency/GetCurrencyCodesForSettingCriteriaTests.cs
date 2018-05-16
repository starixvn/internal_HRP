namespace HRP.Criteria.UnitTests.Currency
{
    using Xunit;

    public class GetCurrencyCodesForSettingCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetCurrencyCodesForSettingCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("Currency_GetCurrencyCodesForSetting", actual);
        }

        [Fact]
        public void IsValid_WhenCall_ReturnTrue()
        {
            // Arrange
            var criteria = new GetCurrencyCodesForSettingCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}