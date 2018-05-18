namespace HRP.Models.UnitTests
{
    using System;
    using HRP.Models.Currency;
    using Xunit;

    public class CurrencyExchangeRateModelTests
    {
        [Fact]
        public void GetExchangeRateDisplay_ExchangeRateIsNull_ReturnNull()
        {
            var model = new CurrencyExchangeRateModel
            {
                ExchangeRate = null
            };

            var result = model.GetExchangeRateDisplay(1);

            Assert.Null(result);
        }

        [Fact]
        public void GetExchangeRateDisplay_ToCurrencyIsVnd_Return0DigitDecimal()
        {
            var vndCurrencyId = 1;
            var model = new CurrencyExchangeRateModel
            {
                ExchangeRate = (decimal)1000.123456,
                ToCurrencyId = vndCurrencyId
            };

            var result = model.GetExchangeRateDisplay(vndCurrencyId);

            Assert.Equal(Math.Round(model.ExchangeRate.Value, 0, MidpointRounding.AwayFromZero), result);
        }

        [Fact]
        public void GetExchangeRateDisplay_ToCurrencyIsNotVnd_Return5DigitDecimal()
        {
            var vndCurrencyId = 1;
            var model = new CurrencyExchangeRateModel
            {
                ExchangeRate = (decimal)1000.123456,
                ToCurrencyId = 2
            };

            var result = model.GetExchangeRateDisplay(vndCurrencyId);

            Assert.Equal(Math.Round(model.ExchangeRate.Value, 5, MidpointRounding.AwayFromZero), result);
        }
    }
}
