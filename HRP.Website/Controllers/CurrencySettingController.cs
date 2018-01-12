namespace HRP.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Filters;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models.Currency;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    [HRPAuthorize]
    public class CurrencySettingController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IBaseService baseService;
        private readonly IOptionsSnapshot<AppSettingsOptions> configuration;

        public CurrencySettingController(
            ISessionService sessionService,
            IBaseService baseService,
            IOptionsSnapshot<AppSettingsOptions> configuration)
        {
            this.sessionService = sessionService;
            this.baseService = baseService;
            this.configuration = configuration;
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.CurrencySetting, Policy = Policy.Read)]
        public IActionResult Index(DateTime? selectedTime)
        {
            if (!selectedTime.HasValue)
            {
                selectedTime = DateTime.Today;
            }

            var criteria = new GetExchangeRateCriteria
            {
                Month = selectedTime.Value.Month,
                Year = selectedTime.Value.Year,
            };

            var viewModel = new CurrencySettingViewModel
            {
                SelectedTime = selectedTime.Value,
                MinMonth = configuration.Value.LaunchMonth,
                CurrencyExchangeRates = baseService.GetData<CurrencyExchangeRateModel>(criteria),
                Currencies = baseService.GetData<CurrencyCodeModel>(new GetActiveCurrencyCodesCriteria()),
                CanUpdate = sessionService.HasPermissionOnFunction(FunctionKey.CurrencySetting, Policy.Host) && selectedTime.Value.Month == DateTime.Today.Month
            };

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.CurrencySetting, Policy = Policy.Host)]
        public IActionResult Save(DateTime selectedTime, IEnumerable<CurrencyExchangeRateModel> exchangeRates)
        {
            var criteria = new UpdateExchangeRatesCriteria
            {
                ExchangeRateMonth = selectedTime,
                UpdatedBy = sessionService.UserId
            };

            var xml = new XElement("Root");
            foreach (var rate in exchangeRates)
            {
                var row = new XElement("Row");
                row.SetAttributeValue(nameof(rate.FromCurrency), rate.FromCurrency);
                row.SetAttributeValue(nameof(rate.ToCurrency), rate.ToCurrency);
                row.SetAttributeValue(nameof(rate.ExchangeRate), rate.ExchangeRate);
                xml.Add(row);
            }

            criteria.ExchangeRates = xml.ToString();
            var result = baseService.ExecuteNonQuery(criteria);

            return Json(result);
        }
    }
}