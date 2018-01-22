namespace HRP.Criteria.UnitTests.Currency
{
    using Xunit;

    public class GetLatestExchangeRatesCriteriaTest
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            var criteria = new GetLatestExchangeRatesCriteria();
            var expected = "Currency_GetLatestExchangeRates";

            var actual = criteria.GetSettingKey();
            Assert.Equal(expected, actual);
        }
    }
}
