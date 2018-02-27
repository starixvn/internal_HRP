namespace HRP.Criteria.UnitTests.Currency
{
    using System;
    using System.Linq;
    using HRP.Models.Currency;
    using Xunit;

    public class UpdateExchangeRatesCriteriaTest
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            var criteria = new UpdateExchangeRatesCriteria(Enumerable.Empty<CurrencyExchangeRateModel>());
            var expected = "Currency_UpdateExchangeRates";

            var actual = criteria.GetSettingKey();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ValidData_ReturnTrue()
        {
            var criteria = new UpdateExchangeRatesCriteria(Enumerable.Empty<CurrencyExchangeRateModel>())
            {
                ExchangeRateMonth = DateTime.Today,
                ExchangeRates = "<root></root>",
                UpdatedBy = 1
            };

            Assert.True(criteria.IsValid());
        }

        [Fact]
        public void IsValid_InvalidUserId_ReturnFalse()
        {
            var criteria = new UpdateExchangeRatesCriteria(Enumerable.Empty<CurrencyExchangeRateModel>())
            {
                ExchangeRateMonth = DateTime.Today,
                ExchangeRates = "<root></root>",
                UpdatedBy = 0
            };

            Assert.False(criteria.IsValid());
        }

        [Fact]
        public void IsValid_InvalidExchangeRatesData_ReturnFalse()
        {
            var criteria = new UpdateExchangeRatesCriteria(Enumerable.Empty<CurrencyExchangeRateModel>())
            {
                ExchangeRateMonth = DateTime.Today,
                ExchangeRates = string.Empty,
                UpdatedBy = 1
            };

            Assert.False(criteria.IsValid());
        }
    }
}