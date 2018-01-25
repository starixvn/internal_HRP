namespace HRP.Website.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Filters;
    using HRP.Core.Services;
    using HRP.Criteria.Company;
    using HRP.Models.DataConfiguration;
    using HRP.Resources;
    using HRP.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class DataConfigurationController : Controller
    {
        private readonly IDataConfigurationService dataConfigurationService;
        private readonly IBaseService baseService;

        public DataConfigurationController(IDataConfigurationService dataConfigurationService,
            IBaseService baseService)
        {
            this.dataConfigurationService = dataConfigurationService;
            this.baseService = baseService;
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Read)]
        public IActionResult Index()
        {
            return View();
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Read)]
        public async Task<IActionResult> GetAllCompanyBVI()
        {
            var companies = await dataConfigurationService.GetAllCompanyBVIAsync();
            var viewModel = new SimpleItemTableViewModel
            {
                FieldName = ViewLang.CompanyBVI,
                Items = companies,
                LinkCreateItem = DataConfigurationActionLink.CreateCompanyBVI,
                LinkCheckExists = DataConfigurationActionLink.CheckCompanyBVIExists
            };

            return PartialView("_SimpleItemTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Read)]
        public async Task<IActionResult> CheckCompanyBVIExists(SimpleItem newItem)
        {
            var isExisted = (await baseService.GetDataAsync<int>(new CheckCompanyExistsCriteria { Code = newItem.ItemName })).Any();

            return Json(isExisted ? string.Format(Message.ItemNameExisted, newItem.ItemName) : "true");
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public IActionResult CreateCompanyBVI()
        {
            var model = new SimpleItem();

            return View(model);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateCompanyBVI(SimpleItem newItem)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateCompanyBVIAsync(newItem);

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = commandResult.Message });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }
    }
}