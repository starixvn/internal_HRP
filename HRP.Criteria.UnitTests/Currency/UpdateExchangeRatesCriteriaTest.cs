
namespace HRP.Criteria.UnitTests.Currency
{
    using System;
    using System.Collections.Generic;
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

        public static IEnumerable<object[]> TestCase()
        {
            yield return new object[] { DateTime.Today, "<root></root>", 1, true };
            yield return new object[] { DateTime.Today, "<root></root>", 0, false };
            yield return new object[] { DateTime.Today, string.Empty, 1, false };
        }

        [Theory, MemberData(nameof(TestCase))]
        public void IsValid_ReturnAsExpected(DateTime exchangeRateMonth, string exchangeRates, int updatedBy, bool expected)
        {
            var criteria = new UpdateExchangeRatesCriteria
            {
                ExchangeRateMonth = exchangeRateMonth,
                ExchangeRates = exchangeRates,
                UpdatedBy = updatedBy
            };

            Assert.Equal(expected, criteria.IsValid());
        }
    }
}
