namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models.Currency;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;

    public class CurrencySettingControllerTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;

        [Fact]
        public void Index_WhenCalled_ReturnCurrentData()
        {
            var currencyExchangeRates = new List<CurrencyExchangeRateModel>
            {
                new CurrencyExchangeRateModel { FromCurrency = "USD", ToCurrency = "VND", ExchangeRate = 22300 },
                new CurrencyExchangeRateModel { FromCurrency = "SGD", ToCurrency = "VND", ExchangeRate = 19300 }
            };

            var currencies = new List<CurrencyCodeModel>
            {
                new CurrencyCodeModel { CurrencyCode = "USD" },
                new CurrencyCodeModel { CurrencyCode = "SGD" },
                new CurrencyCodeModel { CurrencyCode = "VND" },
            };

            var controller = GetController();
            baseService.GetData<CurrencyExchangeRateModel>(Arg.Any<GetExchangeRateCriteria>()).Returns(currencyExchangeRates);
            baseService.GetData<CurrencyCodeModel>(Arg.Any<GetActiveCurrencyCodesCriteria>()).Returns(currencies);
            baseService.GetData<DateTime>(Arg.Any<GetExchageRateMonthsCriteria>()).Returns(new List<DateTime> { DateTime.Today });

            var actual = controller.Index(DateTime.Today);
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<CurrencySettingViewModel>(viewResult.Model);
        }

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId = 1;

            return service;
        }

        private IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private CurrencySettingController GetController()
        {
            baseService = FakeBaseService();
            sessionService = FakeSessionService();
            var controller = new CurrencySettingController(sessionService, baseService);

            return controller;
        }
    }
}