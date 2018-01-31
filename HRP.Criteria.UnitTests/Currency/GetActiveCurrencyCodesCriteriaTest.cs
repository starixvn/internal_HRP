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

        [Fact]
        public void IsValid_ReturnTrue()
        {
            var criteria = new GetActiveCurrencyCodesCriteria();
            Assert.True(criteria.IsValid());
        }
    }
}
