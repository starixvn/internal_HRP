namespace HRP.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
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
    public class CurrencySettingController : BaseController
    {
        private readonly IOptionsSnapshot<AppSettingsOptions> configuration;

        public CurrencySettingController(
            ISessionService sessionService,
            IBaseService baseService,
            IOptionsSnapshot<AppSettingsOptions> configuration) : base(baseService, sessionService)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.CurrencySetting, Policy = Policy.Read)]
        public async Task<IActionResult> Index(DateTime? selectedTime)
        {
            var date = selectedTime ?? DateTime.Today;

            var criteria = new GetExchangeRateCriteria
            {
                Month = date.Month,
                Year = date.Year,
            };

            var viewModel = new CurrencySettingViewModel
            {
                SelectedTime = date,
                MinMonth = configuration.Value.LaunchMonth,
                CurrencyExchangeRates = await baseService.GetDataAsync<CurrencyExchangeRateModel>(criteria),
                Currencies = await baseService.GetDataAsync<CurrencyCodeModel>(new GetActiveCurrencyCodesCriteria()),
                CanUpdate = sessionService.HasPermissionOnFunction(FunctionKey.CurrencySetting, Policy.Host) && date.Month == DateTime.Today.Month
            };

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.CurrencySetting, Policy = Policy.Host)]
        public async Task<IActionResult> Edit(DateTime selectedTime, IEnumerable<CurrencyExchangeRateModel> exchangeRates)
        {
            var criteria = new UpdateExchangeRatesCriteria(exchangeRates)
            {
                ExchangeRateMonth = selectedTime,
                UpdatedBy = sessionService.UserId
            };
           
            var result = await baseService.ExecuteNonQueryAsync(criteria);
            return Json(result);
        }
    }
}