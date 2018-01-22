namespace HRP.Criteria.UnitTests.Currency
{
    using Xunit;

    public class GetActiveCurrencyCodesCriteriaTest
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            var criteria = new GetActiveCurrencyCodesCriteria();
            var expected = "Currency_GetActiveCurrencyCodes";

            var actual = criteria.GetSettingKey();
            Assert.Equal(expected, actual);
        }
    }
}
