namespace HRP.Website.Controllers
{
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Filters;
    using HRP.Models.DataConfiguration;
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

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateCompanyBVIAsync(SimpleItem item)
        {
            var commandResult = await dataConfigurationService.CreateCompanyBVIAsync(item);

            return View(commandResult);
        }
    }
}