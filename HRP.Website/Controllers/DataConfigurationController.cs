namespace HRP.Website.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Filters;
    using HRP.Core.Services;
    using HRP.Models;
    using HRP.Models.DataConfiguration;
    using HRP.Models.DataConfiguration.City;
    using HRP.Models.DataConfiguration.CostCenter;
    using HRP.Models.DataConfiguration.Department;
    using HRP.Models.DataConfiguration.Division;
    using HRP.Models.DataConfiguration.GradeLevel;
    using HRP.Models.DataConfiguration.JobGrade;
    using HRP.Models.DataConfiguration.Office;
    using HRP.Models.DataConfiguration.Section;
    using HRP.Resources;
    using HRP.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    [HRPAuthorize]
    public class DataConfigurationController : BaseController
    {
        private readonly IDataConfigurationService dataConfigurationService;

        public DataConfigurationController(
            IDataConfigurationService dataConfigurationService,
            ISessionService sessionService,
            IBaseService baseService) : base(baseService, sessionService)
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

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = commandResult.Message });
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
                var viewModel = new UpdateSimpleItemViewModel
                {
                    FieldName = ViewLang.CompanyBVI,
                    Item = company,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateCompanyBVI
                };

                return PartialView("_UpdateSimpleItem", viewModel);
            }

            return Content(Message.NotFoundError);
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
            var viewModel = new CreateDivisionViewModel
            {
                CompanyList = await dataConfigurationService.GetCompanyBVISelectListAsync(),
                FieldName = ViewLang.Division,
                LinkCreateItem = DataConfigurationActionLink.CreateDivision
            };

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

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = commandResult.Message });
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
                var viewModel = new UpdateDivisionViewModel
                {
                    FieldName = ViewLang.Division,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateDivision,
                    Division = division
                };

                return PartialView("_UpdateDivision", viewModel);
            }

            return Content(Message.NotFoundError);
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
            var viewModel = new CreateDepartmentViewModel
            {
                CompanyList = await dataConfigurationService.GetCompanyBVISelectListAsync(),
                FieldName = ViewLang.Department,
                LinkCreateItem = DataConfigurationActionLink.CreateDepartment
            };

            return PartialView("_CreateDepartment", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateDepartment(Department department)
        {
            var isExistedItem = await dataConfigurationService.IsExistedDepartmentAsync(department);

            if (isExistedItem)
            {
                var modelName = string.Concat(nameof(department), ".", nameof(department.Code));
                ModelState.AddModelError(modelName, string.Format(Message.ItemNameExisted, department.Code));
            }

            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateDepartmentAsync(department);

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = commandResult.Message });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateDepartment(int id)
        {
            var department = await dataConfigurationService.GetDepartmentByIdAsync(id);

            if (department != null)
            {
                var viewModel = new UpdateDepartmentViewModel
                {
                    FieldName = ViewLang.Department,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateDepartment,
                    Department = department
                };

                return PartialView("_UpdateDepartment", viewModel);
            }

            return Content(Message.NotFoundError);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateDepartment(Department department)
        {
            var isExistedItem = await dataConfigurationService.IsExistedDepartmentAsync(department);

            if (isExistedItem)
            {
                var modelName = string.Concat(nameof(department), ".", nameof(department.Code));
                ModelState.AddModelError(modelName, string.Format(Message.ItemNameExisted, department.Code));
            }

            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.UpdateDepartmentAsync(department);

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = commandResult.Message });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        #endregion Department

        #region Cost Center

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Read)]
        public async Task<IActionResult> GetAllCostCenter()
        {
            var costCenters = await dataConfigurationService.GetAllCostCenterAsync();
            var viewModel = new CostCenterTableViewModel
            {
                CostCenters = costCenters,
                FieldName = ViewLang.CostCenter,
                LinkCreateItem = DataConfigurationActionLink.CreateCostCenter,
                LinkUpdateItem = DataConfigurationActionLink.UpdateCostCenter
            };

            return PartialView("_CostCenterTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateCostCenter()
        {
            var viewModel = new CreateCostCenterViewModel
            {
                CompanyList = await dataConfigurationService.GetCompanyBVISelectListAsync(),
                FieldName = ViewLang.CostCenter,
                LinkCreateItem = DataConfigurationActionLink.CreateCostCenter
            };

            return PartialView("_CreateCostCenter", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateCostCenter(CostCenter costCenter)
        {
            var isExistedItem = await dataConfigurationService.IsExistedCostCenterAsync(costCenter);

            if (isExistedItem)
            {
                var modelName = string.Concat(nameof(costCenter), ".", nameof(costCenter.Code));
                ModelState.AddModelError(modelName, string.Format(Message.ItemNameExisted, costCenter.Code));
            }

            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateCostCenterAsync(costCenter);

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = commandResult.Message });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateCostCenter(int id)
        {
            var costCenter = await dataConfigurationService.GetCostCenterByIdAsync(id);

            if (costCenter != null)
            {
                var viewModel = new UpdateCostCenterViewModel
                {
                    FieldName = ViewLang.CostCenter,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateCostCenter,
                    CostCenter = costCenter
                };

                return PartialView("_UpdateCostCenter", viewModel);
            }

            return Content(Message.NotFoundError);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateCostCenter(CostCenter costCenter)
        {
            var isExistedItem = await dataConfigurationService.IsExistedCostCenterAsync(costCenter);

            if (isExistedItem)
            {
                var modelName = string.Concat(nameof(costCenter), ".", nameof(costCenter.Code));
                ModelState.AddModelError(modelName, string.Format(Message.ItemNameExisted, costCenter.Code));
            }

            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.UpdateCostCenterAsync(costCenter);

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = commandResult.Message });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        #endregion Cost Center

        #region Section

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Read)]
        public async Task<IActionResult> GetAllSection()
        {
            var sections = await dataConfigurationService.GetAllSectionAsync();
            var viewModel = new SectionTableViewModel
            {
                Sections = sections,
                FieldName = ViewLang.Section,
                LinkCreateItem = DataConfigurationActionLink.CreateSection,
                LinkUpdateItem = DataConfigurationActionLink.UpdateSection
            };

            return PartialView("_SectionTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateSection()
        {
            var viewModel = new CreateSectionViewModel
            {
                CompanyList = await dataConfigurationService.GetCompanyBVISelectListAsync(),
                FieldName = ViewLang.Section,
                LinkCreateItem = DataConfigurationActionLink.CreateSection
            };

            return PartialView("_CreateSection", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateSection(Section section)
        {
            var isExistedItem = await dataConfigurationService.IsExistedSectionAsync(section);

            if (isExistedItem)
            {
                var modelName = string.Concat(nameof(section), ".", nameof(section.Code));
                ModelState.AddModelError(modelName, string.Format(Message.ItemNameExisted, section.Code));
            }

            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateSectionAsync(section);

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = commandResult.Message });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateSection(int id)
        {
            var section = await dataConfigurationService.GetSectionByIdAsync(id);

            if (section != null)
            {
                var viewModel = new UpdateSectionViewModel
                {
                    FieldName = ViewLang.Section,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateSection,
                    Section = section
                };

                return PartialView("_UpdateSection", viewModel);
            }

            return Content(Message.NotFoundError);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateSection(Section section)
        {
            var isExistedItem = await dataConfigurationService.IsExistedSectionAsync(section);

            if (isExistedItem)
            {
                var modelName = string.Concat(nameof(section), ".", nameof(section.Code));
                ModelState.AddModelError(modelName, string.Format(Message.ItemNameExisted, section.Code));
            }

            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.UpdateSectionAsync(section);

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = commandResult.Message });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        #endregion Section

        #region Job Category

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Read)]
        public async Task<IActionResult> GetAllJobCategory()
        {
            var jobCategories = await dataConfigurationService.GetAllJobCategoryAsync();
            var viewModel = new SimpleItemTableViewModel
            {
                Items = jobCategories,
                FieldName = ViewLang.JobCategory,
                LinkCreateItem = DataConfigurationActionLink.CreateJobCategory,
                LinkUpdateItem = DataConfigurationActionLink.UpdateJobCategory
            };

            return PartialView("_SimpleItemTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public IActionResult CreateJobCategory()
        {
            var viewModel = new CreateSimpleItemViewModel
            {
                FieldName = ViewLang.JobCategory,
                LinkCreateItem = DataConfigurationActionLink.CreateJobCategory
            };

            return PartialView("_CreateSimpleItem", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateJobCategory(SimpleItem item)
        {
            var isExistedItem = await dataConfigurationService.IsExistedJobCategoryAsync(item);

            if (isExistedItem)
            {
                var modelName = string.Concat(nameof(item), ".", nameof(item.ItemName));
                ModelState.AddModelError(modelName, string.Format(Message.ItemNameExisted, item.ItemName));
            }

            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateJobCategoryAsync(item);

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = commandResult.Message });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateJobCategory(int id)
        {
            var jobCategory = await dataConfigurationService.GetJobCategoryByIdAsync(id);

            if (jobCategory != null)
            {
                var viewModel = new UpdateSimpleItemViewModel
                {
                    FieldName = ViewLang.JobCategory,
                    Item = jobCategory,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateJobCategory
                };

                return PartialView("_UpdateSimpleItem", viewModel);
            }

            return Content(Message.NotFoundError);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateJobCategory(SimpleItem item)
        {
            var isExistedItem = await dataConfigurationService.IsExistedJobCategoryAsync(item);

            if (isExistedItem)
            {
                var modelName = string.Concat(nameof(item), ".", nameof(item.ItemName));
                ModelState.AddModelError(modelName, string.Format(Message.ItemNameExisted, item.ItemName));
            }

            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.UpdateJobCategoryAsync(item);

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = commandResult.Message });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        #endregion Job Category

        #region Grade Level

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Read)]
        public async Task<IActionResult> GetAllGradeLevel()
        {
            var gradeLevelList = await dataConfigurationService.GetAllGradeLevelAsync();
            var viewModel = new GradeLevelTableViewModel
            {
                GradeLevelList = gradeLevelList,
                FieldName = ViewLang.GradeLevel,
                LinkCreateItem = DataConfigurationActionLink.CreateGradeLevel,
                LinkUpdateItem = DataConfigurationActionLink.UpdateGradeLevel
            };

            return PartialView("_GradeLevelTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateGradeLevel()
        {
            var viewModel = new CreateGradeLevelViewModel
            {
                JobCategoryList = await dataConfigurationService.GetJobCategorySelectListAsync(),
                FieldName = ViewLang.GradeLevel,
                LinkCreateItem = DataConfigurationActionLink.CreateGradeLevel
            };

            return PartialView("_CreateGradeLevel", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateGradeLevel(GradeLevel gradeLevel)
        {
            var isExistedItem = await dataConfigurationService.IsExistedGradeLevelAsync(gradeLevel);

            if (isExistedItem)
            {
                var modelName = string.Concat(nameof(gradeLevel), ".", nameof(gradeLevel.Name));
                ModelState.AddModelError(modelName, string.Format(Message.ItemNameExisted, gradeLevel.Name));
            }

            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateGradeLevelAsync(gradeLevel);

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = commandResult.Message });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateGradeLevel(int id)
        {
            var gradeLevel = await dataConfigurationService.GetGradeLevelByIdAsync(id);

            if (gradeLevel != null)
            {
                var viewModel = new UpdateGradeLevelViewModel
                {
                    FieldName = ViewLang.GradeLevel,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateGradeLevel,
                    GradeLevel = gradeLevel
                };

                return PartialView("_UpdateGradeLevel", viewModel);
            }

            return Content(Message.NotFoundError);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateGradeLevel(GradeLevel gradeLevel)
        {
            var isExistedItem = await dataConfigurationService.IsExistedGradeLevelAsync(gradeLevel);

            if (isExistedItem)
            {
                var modelName = string.Concat(nameof(gradeLevel), ".", nameof(gradeLevel.Name));
                ModelState.AddModelError(modelName, string.Format(Message.ItemNameExisted, gradeLevel.Name));
            }

            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.UpdateGradeLevelAsync(gradeLevel);

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = commandResult.Message });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        #endregion Grade Level

        #region Job Grade

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Read)]
        public async Task<IActionResult> GetAllJobGrade()
        {
            var jobGradeList = await dataConfigurationService.GetAllJobGradelAsync();
            var viewModel = new JobGradeTableViewModel
            {
                JobGradeList = jobGradeList,
                FieldName = ViewLang.JobGrade,
                LinkCreateItem = DataConfigurationActionLink.CreateJobGrade,
                LinkUpdateItem = DataConfigurationActionLink.UpdateJobGrade
            };

            return PartialView("_JobGradeTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateJobGrade()
        {
            var viewModel = new CreateJobGradeViewModel
            {
                JobCategoryList = await dataConfigurationService.GetJobCategorySelectListAsync(),
                FieldName = ViewLang.JobGrade,
                LinkCreateItem = DataConfigurationActionLink.CreateJobGrade
            };

            return PartialView("_CreateJobGrade", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateJobGrade(JobGrade jobGrade)
        {
            var isExistedItem = await dataConfigurationService.IsExistedJobGradeAsync(jobGrade);

            if (isExistedItem)
            {
                var modelName = string.Concat(nameof(jobGrade), ".", nameof(jobGrade.Name));
                ModelState.AddModelError(modelName, string.Format(Message.ItemNameExisted, jobGrade.Name));
            }

            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateJobGradeAsync(jobGrade);

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = commandResult.Message });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateJobGrade(int id)
        {
            var jobGrade = await dataConfigurationService.GetJobGradeByIdAsync(id);

            if (jobGrade != null)
            {
                var viewModel = new UpdateJobGradeViewModel
                {
                    FieldName = ViewLang.JobGrade,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateJobGrade,
                    JobGrade = jobGrade
                };

                return PartialView("_UpdateJobGrade", viewModel);
            }

            return Content(Message.NotFoundError);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateJobGrade(JobGrade jobGrade)
        {
            var isExistedItem = await dataConfigurationService.IsExistedJobGradeAsync(jobGrade);

            if (isExistedItem)
            {
                var modelName = string.Concat(nameof(jobGrade), ".", nameof(jobGrade.Name));
                ModelState.AddModelError(modelName, string.Format(Message.ItemNameExisted, jobGrade.Name));
            }

            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.UpdateJobGradeAsync(jobGrade);

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = commandResult.Message });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        #endregion Job Grade

        #region City

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Read)]
        public async Task<IActionResult> GetAllCity()
        {
            var cities = await dataConfigurationService.GetAllCityAsync();
            var viewModel = new CityTableViewModel
            {
                Cities = cities,
                FieldName = ViewLang.City,
                LinkCreateItem = DataConfigurationActionLink.CreateCity,
                LinkUpdateItem = DataConfigurationActionLink.UpdateCity
            };

            return PartialView("_CityTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateCity()
        {
            var viewModel = new CreateCityViewModel
            {
                CountryList = await dataConfigurationService.GetCountrySelectListAsync(),
                FieldName = ViewLang.City,
                LinkCreateItem = DataConfigurationActionLink.CreateCity
            };

            return PartialView("_CreateCity", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateCity(City city)
        {
            var isExistedItem = await dataConfigurationService.IsExistedCityAsync(city);

            if (isExistedItem)
            {
                var modelName = string.Concat(nameof(city), ".", nameof(city.Code));
                ModelState.AddModelError(modelName, string.Format(Message.ItemNameExisted, city.Code));
            }

            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateCityAsync(city);

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = commandResult.Message });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateCity(int id)
        {
            var city = await dataConfigurationService.GetCityByIdAsync(id);

            if (city != null)
            {
                var viewModel = new UpdateCityViewModel
                {
                    FieldName = ViewLang.City,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateCity,
                    City = city
                };

                return PartialView("_UpdateCity", viewModel);
            }

            return Content(Message.NotFoundError);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateCity(City city)
        {
            var isExistedItem = await dataConfigurationService.IsExistedCityAsync(city);

            if (isExistedItem)
            {
                var modelName = string.Concat(nameof(city), ".", nameof(city.Code));
                ModelState.AddModelError(modelName, string.Format(Message.ItemNameExisted, city.Code));
            }

            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.UpdateCityAsync(city);

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = commandResult.Message });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        #endregion City

        #region Office

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Read)]
        public async Task<IActionResult> GetAllOffice()
        {
            var offices = await dataConfigurationService.GetAllOfficeAsync();
            var viewModel = new OfficeTableViewModel
            {
                Offices = offices,
                FieldName = ViewLang.Office,
                LinkCreateItem = DataConfigurationActionLink.CreateOffice,
                LinkUpdateItem = DataConfigurationActionLink.UpdateOffice
            };

            return PartialView("_OfficeTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateOffice()
        {
            var viewModel = new CreateOfficeViewModel
            {
                CountryList = await dataConfigurationService.GetCountrySelectListAsync(),
                FieldName = ViewLang.Office,
                LinkCreateItem = DataConfigurationActionLink.CreateOffice
            };

            return PartialView("_CreateOffice", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateOffice(Office office)
        {
            var isExistedItem = await dataConfigurationService.IsExistedOfficeAsync(office);

            if (isExistedItem)
            {
                var modelName = string.Concat(nameof(office), ".", nameof(office.Code));
                ModelState.AddModelError(modelName, string.Format(Message.ItemNameExisted, office.Code));
            }

            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateOfficeAsync(office);

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = commandResult.Message });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateOffice(int id)
        {
            var office = await dataConfigurationService.GetOfficeByIdAsync(id);

            if (office != null)
            {
                var viewModel = new UpdateOfficeViewModel
                {
                    FieldName = ViewLang.Office,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateOffice,
                    Office = office
                };

                return PartialView("_UpdateOffice", viewModel);
            }

            return Content(Message.NotFoundError);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateOffice(Office office)
        {
            var isExistedItem = await dataConfigurationService.IsExistedOfficeAsync(office);

            if (isExistedItem)
            {
                var modelName = string.Concat(nameof(office), ".", nameof(office.Code));
                ModelState.AddModelError(modelName, string.Format(Message.ItemNameExisted, office.Code));
            }

            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.UpdateOfficeAsync(office);

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = commandResult.Message });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        #endregion Office

        #region Commons

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> GetDivisionSelectListByCompanyId(int companyId)
        {
            var divisionList = await dataConfigurationService.GetDivisionSelectListByCompanyIdAsync(companyId);

            return PartialView("_SelectListItem", divisionList);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> GetDepartmentSelectListByDivisionId(int divisionId)
        {
            var departmentList = await dataConfigurationService.GetDepartmentSelectListByDivisionIdAsync(divisionId);

            return PartialView("_SelectListItem", departmentList);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> GetCostCenterSelectListByDepartmentId(int departmentId)
        {
            var costCenterList = await dataConfigurationService.GetCostCenterSelectListByDepartmentIdAsync(departmentId);

            return PartialView("_SelectListItem", costCenterList);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> GetSectionSelectListByCostCenterId(int costCenterId)
        {
            var sectionList = await dataConfigurationService.GetSectionSelectListByCostCenterIdAsync(costCenterId);

            return PartialView("_SelectListItem", sectionList);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> GetGradeLevelSelectListByJobCategoryId(int jobCategoryId)
        {
            var gradeLevelList = await dataConfigurationService.GetGradeLevelSelectListByJobCateogryIdAsync(jobCategoryId);

            return PartialView("_SelectListItem", gradeLevelList);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> GetCitySelectListByCountryId(int countryId)
        {
            var cityList = await dataConfigurationService.GetCitySelectListByCountryIdAsync(countryId);

            return PartialView("_SelectListItem", cityList);
        }

        private static string UppercaseFirstLetter(string s)
        {
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);

            return new string(a);
        }

        #endregion Commons
    }
}