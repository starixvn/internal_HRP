namespace HRP.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Filters;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models.Currency;
    using Microsoft.AspNetCore.Mvc;

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
        [HRPPermission(FunctionKey = FunctionKey.CurrencySetting, Policy = Policy.Read)]
        public IActionResult Index(DateTime? selectedTime)
        {
            var criteria = new GetExchangeRateCriteria
            {
                Month = selectedTime.HasValue ? selectedTime.Value.Month : DateTime.Today.Month,
                Year = selectedTime.HasValue ? selectedTime.Value.Year : DateTime.Today.Year,
            };

            var viewModel = new CurrencySettingViewModel
            {
                SelectedTime = selectedTime ?? DateTime.Today,
                CurrencyExchangeRates = baseService.GetData<CurrencyExchangeRateModel>(criteria),
                Currencies = baseService.GetData<CurrencyCodeModel>(new GetActiveCurrencyCodesCriteria()),
                Months = baseService.GetData<DateTime>(new GetExchageRateMonthsCriteria())
            };

            if (!viewModel.Months.Any(x => x.Month == DateTime.Today.Month && x.Year == DateTime.Today.Year))
            {
                var list = viewModel.Months.ToList();
                list.Add(DateTime.Today);

                viewModel.Months = list.OrderByDescending(x => x);
            }

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