namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models.Currency;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
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
            sessionService.HasPermissionOnFunction(FunctionKey.CurrencySetting, Policy.Host).Returns(true);

            var actual = controller.Index(DateTime.Today);
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<CurrencySettingViewModel>(viewResult.Model);
        }

        [Fact]
        public void Edit_WhenCalled_ReturnExpectedCommandResult()
        {
            var exchangeRates = new List<CurrencyExchangeRateModel>
            {
                new CurrencyExchangeRateModel
                {
                    FromCurrency = "USD",
                    ToCurrency = "VND",
                    ExchangeRate = 22500
                }
            };

            var controller = GetController();
            baseService.ExecuteNonQuery(Arg.Any<UpdateExchangeRatesCriteria>()).Returns(new CommandResult { Code = 0, Message = string.Empty });

            // Act
            var actual = controller.Edit(DateTime.Today, exchangeRates);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var commandResult = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.True(commandResult.IsSuccessful);
        }

        private static ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId = 1;

            return service;
        }

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private IOptionsSnapshot<AppSettingsOptions> FakeConfiguration()
        {
            var appSettingsOptions = new AppSettingsOptions
            {
                SessionTimeout = 60,
                EnableEncrypt = true,
                CertificateFilePath = "D=\\Test.pfx",
                PasswordToSignTheCertificateFile = "123456",
                Domain = "http=//localhost=56977",
                LaunchMonth = DateTime.Today
            };

            var configuration = Substitute.For<IOptionsSnapshot<AppSettingsOptions>>();
            configuration.Value.Returns(appSettingsOptions);

            return configuration;
        }

        private CurrencySettingController GetController()
        {
            baseService = FakeBaseService();
            sessionService = FakeSessionService();
            var configuration = FakeConfiguration();
            var controller = new CurrencySettingController(sessionService, baseService, configuration);

            return controller;
        }
    }
}