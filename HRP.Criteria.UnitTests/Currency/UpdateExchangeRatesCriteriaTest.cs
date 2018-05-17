namespace HRP.Criteria.UnitTests.Currency
{
    using System;
    using Xunit;

    public class UpdateExchangeRatesCriteriaTest
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            var criteria = new UpdateExchangeRatesCriteria();
            var expected = "Currency_UpdateExchangeRates";

            var actual = criteria.GetSettingKey();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            var criteria = new UpdateExchangeRatesCriteria
            {
                ExchangeRateMonth = DateTime.Today,
                ExchangeRates = "<root></root>",
                UpdatedBy = 1
            };

            Assert.True(criteria.IsValid());
        }

        [Theory]
        [InlineData(0, "<root></root>")]
        [InlineData(1, "")]
        [InlineData(0, "")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int userId, string exchangeRates)
        {
            var criteria = new UpdateExchangeRatesCriteria
            {
                ExchangeRateMonth = DateTime.Today,
                ExchangeRates = exchangeRates,
                UpdatedBy = userId
            };

            Assert.False(criteria.IsValid());
        }
    }
}