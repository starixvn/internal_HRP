namespace HRP.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Extensions;
    using HRP.Core.Filters;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.MonthlyPayroll;
    using HRP.Models.Currency;
    using HRP.Resources;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

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
                Currencies = await baseService.GetDataAsync<CurrencyCodeModel>(new GetCurrencyCodesForSettingCriteria { Month = date }),
                CanUpdate = sessionService.HasPermissionOnFunction(FunctionKey.CurrencySetting, Policy.Host) && date.Month == DateTime.Today.Month && date.Year == DateTime.Today.Year
            };

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.CurrencySetting, Policy = Policy.Host)]
        public async Task<IActionResult> Edit(DateTime selectedTime, IEnumerable<CurrencyExchangeRateModel> exchangeRates)
        {
            var criteria = new UpdateExchangeRatesCriteria
            {
                ExchangeRateMonth = selectedTime,
                ExchangeRates = exchangeRates.ToXML().ToString(),
                UpdatedBy = sessionService.UserId
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(commandResult);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.CurrencySetting, Policy = Policy.Read)]
        public async Task<IActionResult> IsPayrollExistedForCurrentMonth()
        {
            var result = await baseService.GetDataAsync<string>(new GetCompaniesHavePayrollForCurrentMonthCriteria());
            var message = result.Any() ? string.Format(Message.UpdateCurrencySettingWarningMessage, string.Join(", ", result)) : string.Empty;

            return Json(new { hasPayroll = result.Any(), message });
        }
    }
}