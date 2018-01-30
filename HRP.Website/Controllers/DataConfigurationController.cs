namespace HRP.Website.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Filters;
    using HRP.Models;
    using HRP.Models.DataConfiguration;
    using HRP.Models.DataConfiguration.Department;
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
            var fieldList = dataConfigurationService.GetDataForIndexPage();

            return View(fieldList);
        }

        #region CompanyBVI

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Read)]
        public async Task<IActionResult> GetAllCompanyBVI()
        {
            var companies = await dataConfigurationService.GetAllCompanyBVIAsync();
            var viewModel = new SimpleItemTableViewModel
            {
                Items = companies,
                FieldName = ViewLang.CompanyBVI,
                LinkCreateItem = DataConfigurationActionLink.CreateCompanyBVI,
                LinkUpdateItem = DataConfigurationActionLink.UpdateCompanyBVI
            };

            return PartialView("_SimpleItemTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public IActionResult CreateCompanyBVI()
        {
            var viewModel = new CreateSimpleItemViewModel
            {
                FieldName = ViewLang.CompanyBVI,
                LinkCreateItem = DataConfigurationActionLink.CreateCompanyBVI
            };

            return PartialView("_CreateSimpleItem", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateCompanyBVI(SimpleItem item)
        {
            var isExistedItem = await dataConfigurationService.IsExistedCompanyBVIAsync(item);

            if (isExistedItem)
            {
                var modelName = string.Concat(nameof(item), ".", nameof(item.ItemName));
                ModelState.AddModelError(modelName, string.Format(Message.ItemNameExisted, item.ItemName));
            }

            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateCompanyBVIAsync(item);

                return Json(new { isSuccessful = commandResult.IsSuccessful, code = commandResult.Code, message = commandResult.Message });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateCompanyBVI(int id)
        {
            var company = await dataConfigurationService.GetCompanyBVIByIdAsync(id);

            if (company != null)
            {
                var viewModal = new UpdateSimpleItemViewModel
                {
                    FieldName = ViewLang.CompanyBVI,
                    Item = company,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateCompanyBVI
                };

                return PartialView("_UpdateSimpleItem", viewModal);
            }

            return Content("404 not found");
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateCompanyBVI(SimpleItem item)
        {
            var isExistedItem = await dataConfigurationService.IsExistedCompanyBVIAsync(item);

            if (isExistedItem)
            {
                var modelName = string.Concat(nameof(item), ".", nameof(item.ItemName));
                ModelState.AddModelError(modelName, string.Format(Message.ItemNameExisted, item.ItemName));
            }

            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.UpdateCompanyBVIAsync(item);

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = commandResult.Message });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        #endregion CompanyBVI

        #region Division

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Read)]
        public async Task<IActionResult> GetAllDivision()
        {
            var divisions = await dataConfigurationService.GetAllDivisionAsync();
            var viewModel = new DivisionTableViewModel
            {
                Divisions = divisions,
                FieldName = ViewLang.Division,
                LinkCreateItem = DataConfigurationActionLink.CreateDivision,
                LinkUpdateItem = DataConfigurationActionLink.UpdateDivision
            };

            return PartialView("_DivisionTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateDivision()
        {
            var viewModel = await dataConfigurationService.GetDataForCreateDivisionAsync();
            viewModel.FieldName = ViewLang.Division;
            viewModel.LinkCreateItem = DataConfigurationActionLink.CreateDivision;

            return PartialView("_CreateDivision", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateDivision(Division division)
        {
            var isExistedItem = await dataConfigurationService.IsExistedDivisionAsync(division);

            if (isExistedItem)
            {
                var modelName = string.Concat(nameof(division), ".", nameof(division.Code));
                ModelState.AddModelError(modelName, string.Format(Message.ItemNameExisted, division.Code));
            }

            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateDivisionAsync(division);

                return Json(new { isSuccessful = commandResult.IsSuccessful, code = commandResult.Code, message = commandResult.Message });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateDivision(int id)
        {
            var division = await dataConfigurationService.GetDivisionByIdAsync(id);

            if (division != null)
            {
                var viewModal = new UpdateDivisionViewModel
                {
                    FieldName = ViewLang.Division,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateDivision,
                    Division = division
                };

                return PartialView("_UpdateDivision", viewModal);
            }

            return Content("404 not found");
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateDivision(Division division)
        {
            var isExistedItem = await dataConfigurationService.IsExistedDivisionAsync(division);

            if (isExistedItem)
            {
                var modelName = string.Concat(nameof(division), ".", nameof(division.Code));
                ModelState.AddModelError(modelName, string.Format(Message.ItemNameExisted, division.Code));
            }

            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.UpdateDivisionAsync(division);

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = commandResult.Message });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        #endregion Division

        #region Department

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Read)]
        public async Task<IActionResult> GetAllDepartment()
        {
            var departments = await dataConfigurationService.GetAllDepartmentAsync();
            var viewModel = new DepartmentTableViewModel
            {
                Departments = departments,
                FieldName = ViewLang.Department,
                LinkCreateItem = DataConfigurationActionLink.CreateDepartment,
                LinkUpdateItem = DataConfigurationActionLink.UpdateDepartment
            };

            return PartialView("_DepartmentTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateDepartment()
        {
            var viewModel = await dataConfigurationService.GetDataForCreateDepartmentAsync();
            viewModel.FieldName = ViewLang.Department;
            viewModel.LinkCreateItem = DataConfigurationActionLink.CreateDepartment;

            return PartialView("_CreateDepartment", viewModel);
        }

        #endregion Department

        private static string UppercaseFirstLetter(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);

            return new string(a);
        }
    }
}