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
    }
}