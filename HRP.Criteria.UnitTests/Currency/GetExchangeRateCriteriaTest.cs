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

        [Fact]
        public void IsValid_ValidData_ReturnTrue()
        {
            var criteria = new GetExchangeRateCriteria
            {
                Month = 1,
                Year = 2012
            };

            var actual = criteria.IsValid();
            Assert.True(actual);
        }

        [Fact]
        public void IsValid_InvalidMonth_ReturnFalse()
        {
            var criteria = new GetExchangeRateCriteria
            {
                Month = 0,
                Year = 2012
            };

            var actual = criteria.IsValid();
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_InvalidYear_ReturnFalse()
        {
            var criteria = new GetExchangeRateCriteria
            {
                Month = 1,
                Year = 0
            };

            var actual = criteria.IsValid();
            Assert.False(actual);
        }
    }
}