
namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class GetExchangeRateCriteriaTest
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            var criteria = new GetExchangeRateCriteria();
            var expected = "Currency_GetExchangeRates";

            var actual = criteria.GetSettingKey();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 2012, true)]
        [InlineData(0, 2012, false)]
        [InlineData(1, 0, false)]
        [InlineData(13, 2012, false)]
        public void IsValid_ReturnAsExpected(int month, int year, bool expected)
        {
            var criteria = new GetExchangeRateCriteria {
                Month = month,
                Year = year
            };

            var actual = criteria.IsValid();
            Assert.Equal(expected, actual);
        }
    }
}
