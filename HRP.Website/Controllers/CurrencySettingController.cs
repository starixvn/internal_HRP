namespace HRP.Website.Controllers
{
    using System;
    using System.Linq;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Filters;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models.Currency;
    using Microsoft.AspNetCore.Mvc;

    [HRPPermission(FunctionKey = FunctionKey.CurrencySetting, Policy = Policy.Read)]
    public class CurrencySettingController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IBaseService baseService;

        public CurrencySettingController(ISessionService sessionService, IBaseService baseService)
        {
            this.sessionService = sessionService;
            this.baseService = baseService;
        }

        [HttpGet]
        public IActionResult Index(int? month, int? year)
        {
            var criteria = new GetExchangeRateCriteria {
                Month = month ?? DateTime.Today.Month,
                Year = year ?? DateTime.Today.Year,
            };

            var viewModel = new CurrencySettingViewModel
            {
                Month = month ?? DateTime.Today.Month,
                Year = year ?? DateTime.Today.Year,
                CurrencyExchangeRates = baseService.GetData<CurrencyExchangeRateModel>(criteria),
                Currencies = baseService.GetData<CurrencyCodeModel>(new GetActiveCurrencyCodesCriteria()),
                Months = baseService.GetData<DateTime>(new GetExchageRateMonthsCriteria())
            };

            if (!viewModel.Months.Any(x => x.Month == DateTime.Today.Month && x.Year == DateTime.Today.Year))
            {
                var list = viewModel.Months.ToList();
                list.Add(DateTime.Today);
                list.Add(DateTime.Today.AddMonths(-1));

                viewModel.Months = list;
            }

            return View(viewModel);
        }
    }
}