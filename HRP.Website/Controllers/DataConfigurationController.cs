namespace HRP.Website.Controllers
{
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Filters;
    using HRP.Models.DataConfiguration;
    using HRP.Resources;
    using HRP.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class DataConfigurationController : Controller
    {
        private readonly IDataConfigurationService dataConfigurationService;

        public DataConfigurationController(IDataConfigurationService dataConfigurationService)
        {
            this.dataConfigurationService = dataConfigurationService;
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
                LinkCreateItem = DataConfigurationActionLink.CreateCompanyBVI
            };

            return PartialView("_SimpleItemTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public IActionResult CreateCompanyBVI()
        {
            var model = new SimpleItem();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateCompanyBVI(SimpleItem newItem)
        {
            var commandResult = await dataConfigurationService.CreateCompanyBVIAsync(newItem);

            return View(commandResult);
        }
    }
}