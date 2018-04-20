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
    using HRP.Services;
    using Microsoft.AspNetCore.Mvc;

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
            var viewModel = new IndexViewModel
            {
                Fields = dataConfigurationService.GetDataForIndexPage()
            };

            return View(viewModel);
        }

        #region CompanyBVI

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Read)]
        public async Task<IActionResult> GetAllCompanyBVI()
        {
            var companies = await dataConfigurationService.GetAllCompanyBVIAsync();
            var viewModel = new SimpleItemTableViewModel
            {
                Items = companies,
                Field = new Field
                {
                    FieldName = ViewLang.CompanyBVI,
                    LinkCreateItem = DataConfigurationActionLink.CreateCompanyBVI,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateCompanyBVI
                },
                IsHost = sessionService.HasPermissionOnFunction(FunctionKey.DataConfiguration, Policy.Host)
            };

            return PartialView("_SimpleItemTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public IActionResult CreateCompanyBVI()
        {
            var viewModel = new CreateSimpleItemViewModel
            {
                Field = new Field
                {
                    FieldName = ViewLang.CompanyBVI,
                    LinkCreateItem = DataConfigurationActionLink.CreateCompanyBVI
                }
            };

            return PartialView("_CreateSimpleItem", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateCompanyBVI(SimpleItem item)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateCompanyBVIAsync(item);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = string.Format(Message.ItemCreatedSuccessful, item.ItemName) });
                }
                else
                {
                    var modelName = string.Concat(nameof(item), ".", nameof(item.ItemName));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
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
                    Item = company,
                    Field = new Field
                    {
                        FieldName = ViewLang.CompanyBVI,
                        LinkUpdateItem = DataConfigurationActionLink.UpdateCompanyBVI
                    }
                };

                return PartialView("_UpdateSimpleItem", viewModel);
            }

            return Content(Message.NotFoundError);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateCompanyBVI(SimpleItem item)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.UpdateCompanyBVIAsync(item);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = Message.ItemUpdatedSuccessful });
                }
                else
                {
                    var modelName = string.Concat(nameof(item), ".", nameof(item.ItemName));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
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
                Field = new Field
                {
                    FieldName = ViewLang.Division,
                    LinkCreateItem = DataConfigurationActionLink.CreateDivision,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateDivision
                },
                IsHost = sessionService.HasPermissionOnFunction(FunctionKey.DataConfiguration, Policy.Host)
            };

            return PartialView("_DivisionTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateDivision()
        {
            var viewModel = new CreateDivisionViewModel
            {
                CompanyList = await dataConfigurationService.GetCompanyBVISelectListAsync(),
                Field = new Field
                {
                    FieldName = ViewLang.Division,
                    LinkCreateItem = DataConfigurationActionLink.CreateDivision
                }
            };

            return PartialView("_CreateDivision", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateDivision(Division division)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateDivisionAsync(division);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = string.Format(Message.ItemCreatedSuccessful, division.Code) });
                }
                else
                {
                    var modelName = string.Concat(nameof(division), ".", nameof(division.Code));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
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
                    Division = division,
                    Field = new Field
                    {
                        FieldName = ViewLang.Division,
                        LinkUpdateItem = DataConfigurationActionLink.UpdateDivision,
                    }
                };

                return PartialView("_UpdateDivision", viewModel);
            }

            return Content(Message.NotFoundError);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateDivision(Division division)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.UpdateDivisionAsync(division);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = Message.ItemUpdatedSuccessful });
                }
                else
                {
                    var modelName = string.Concat(nameof(division), ".", nameof(division.Code));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
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
                Field = new Field
                {
                    FieldName = ViewLang.Department,
                    LinkCreateItem = DataConfigurationActionLink.CreateDepartment,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateDepartment
                },
                IsHost = sessionService.HasPermissionOnFunction(FunctionKey.DataConfiguration, Policy.Host)
            };

            return PartialView("_DepartmentTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateDepartment()
        {
            var viewModel = new CreateDepartmentViewModel
            {
                CompanyList = await dataConfigurationService.GetCompanyBVISelectListAsync(),
                Field = new Field
                {
                    FieldName = ViewLang.Department,
                    LinkCreateItem = DataConfigurationActionLink.CreateDepartment
                }
            };

            return PartialView("_CreateDepartment", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateDepartment(Department department)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateDepartmentAsync(department);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = string.Format(Message.ItemCreatedSuccessful, department.Code) });
                }
                else
                {
                    var modelName = string.Concat(nameof(department), ".", nameof(department.Code));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
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
                    Department = department,
                    Field = new Field
                    {
                        FieldName = ViewLang.Department,
                        LinkUpdateItem = DataConfigurationActionLink.UpdateDepartment
                    }
                };

                return PartialView("_UpdateDepartment", viewModel);
            }

            return Content(Message.NotFoundError);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateDepartment(Department department)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.UpdateDepartmentAsync(department);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = Message.ItemUpdatedSuccessful });
                }
                else
                {
                    var modelName = string.Concat(nameof(department), ".", nameof(department.Code));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
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
                Field = new Field
                {
                    FieldName = ViewLang.CostCenter,
                    LinkCreateItem = DataConfigurationActionLink.CreateCostCenter,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateCostCenter
                },
                IsHost = sessionService.HasPermissionOnFunction(FunctionKey.DataConfiguration, Policy.Host)
            };

            return PartialView("_CostCenterTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateCostCenter()
        {
            var viewModel = new CreateCostCenterViewModel
            {
                CompanyList = await dataConfigurationService.GetCompanyBVISelectListAsync(),
                Field = new Field
                {
                    FieldName = ViewLang.CostCenter,
                    LinkCreateItem = DataConfigurationActionLink.CreateCostCenter
                }
            };

            return PartialView("_CreateCostCenter", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateCostCenter(CostCenter costCenter)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateCostCenterAsync(costCenter);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = string.Format(Message.ItemCreatedSuccessful, costCenter.Code) });
                }
                else
                {
                    var modelName = string.Concat(nameof(costCenter), ".", nameof(costCenter.Code));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
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
                    CostCenter = costCenter,
                    Field = new Field
                    {
                        FieldName = ViewLang.CostCenter,
                        LinkUpdateItem = DataConfigurationActionLink.UpdateCostCenter,
                    }
                };

                return PartialView("_UpdateCostCenter", viewModel);
            }

            return Content(Message.NotFoundError);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateCostCenter(CostCenter costCenter)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.UpdateCostCenterAsync(costCenter);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = Message.ItemUpdatedSuccessful });
                }
                else
                {
                    var modelName = string.Concat(nameof(costCenter), ".", nameof(costCenter.Code));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
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
                Field = new Field
                {
                    FieldName = ViewLang.Section,
                    LinkCreateItem = DataConfigurationActionLink.CreateSection,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateSection
                },
                IsHost = sessionService.HasPermissionOnFunction(FunctionKey.DataConfiguration, Policy.Host)
            };

            return PartialView("_SectionTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateSection()
        {
            var viewModel = new CreateSectionViewModel
            {
                CompanyList = await dataConfigurationService.GetCompanyBVISelectListAsync(),
                Field = new Field
                {
                    FieldName = ViewLang.Section,
                    LinkCreateItem = DataConfigurationActionLink.CreateSection
                }
            };

            return PartialView("_CreateSection", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateSection(Section section)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateSectionAsync(section);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = string.Format(Message.ItemCreatedSuccessful, section.Code) });
                }
                else
                {
                    var modelName = string.Concat(nameof(section), ".", nameof(section.Code));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
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
                    Section = section,
                    Field = new Field
                    {
                        FieldName = ViewLang.Section,
                        LinkUpdateItem = DataConfigurationActionLink.UpdateSection,
                    }
                };

                return PartialView("_UpdateSection", viewModel);
            }

            return Content(Message.NotFoundError);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateSection(Section section)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.UpdateSectionAsync(section);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = Message.ItemUpdatedSuccessful });
                }
                else
                {
                    var modelName = string.Concat(nameof(section), ".", nameof(section.Code));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
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
                Field = new Field
                {
                    FieldName = ViewLang.JobCategory,
                    LinkCreateItem = DataConfigurationActionLink.CreateJobCategory,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateJobCategory
                },
                IsHost = sessionService.HasPermissionOnFunction(FunctionKey.DataConfiguration, Policy.Host)
            };

            return PartialView("_SimpleItemTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public IActionResult CreateJobCategory()
        {
            var viewModel = new CreateSimpleItemViewModel
            {
                Field = new Field
                {
                    FieldName = ViewLang.JobCategory,
                    LinkCreateItem = DataConfigurationActionLink.CreateJobCategory
                }
            };

            return PartialView("_CreateSimpleItem", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateJobCategory(SimpleItem item)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateJobCategoryAsync(item);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = string.Format(Message.ItemCreatedSuccessful, item.ItemName) });
                }
                else
                {
                    var modelName = string.Concat(nameof(item), ".", nameof(item.ItemName));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
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
                    Item = jobCategory,
                    Field = new Field
                    {
                        FieldName = ViewLang.JobCategory,
                        LinkUpdateItem = DataConfigurationActionLink.UpdateJobCategory
                    }
                };

                return PartialView("_UpdateSimpleItem", viewModel);
            }

            return Content(Message.NotFoundError);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateJobCategory(SimpleItem item)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.UpdateJobCategoryAsync(item);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = Message.ItemUpdatedSuccessful });
                }
                else
                {
                    var modelName = string.Concat(nameof(item), ".", nameof(item.ItemName));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
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
                Field = new Field
                {
                    FieldName = ViewLang.GradeLevel,
                    LinkCreateItem = DataConfigurationActionLink.CreateGradeLevel,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateGradeLevel
                },
                IsHost = sessionService.HasPermissionOnFunction(FunctionKey.DataConfiguration, Policy.Host)
            };

            return PartialView("_GradeLevelTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateGradeLevel()
        {
            var viewModel = new CreateGradeLevelViewModel
            {
                JobCategoryList = await dataConfigurationService.GetJobCategorySelectListAsync(),
                Field = new Field
                {
                    FieldName = ViewLang.GradeLevel,
                    LinkCreateItem = DataConfigurationActionLink.CreateGradeLevel
                }
            };

            return PartialView("_CreateGradeLevel", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateGradeLevel(GradeLevel gradeLevel)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateGradeLevelAsync(gradeLevel);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = string.Format(Message.ItemCreatedSuccessful, gradeLevel.Name) });
                }
                else
                {
                    var modelName = string.Concat(nameof(gradeLevel), ".", nameof(gradeLevel.Name));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
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
                    GradeLevel = gradeLevel,
                    Field = new Field
                    {
                        FieldName = ViewLang.GradeLevel,
                        LinkUpdateItem = DataConfigurationActionLink.UpdateGradeLevel,
                    }
                };

                return PartialView("_UpdateGradeLevel", viewModel);
            }

            return Content(Message.NotFoundError);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateGradeLevel(GradeLevel gradeLevel)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.UpdateGradeLevelAsync(gradeLevel);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = Message.ItemUpdatedSuccessful });
                }
                else
                {
                    var modelName = string.Concat(nameof(gradeLevel), ".", nameof(gradeLevel.Name));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
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
                Field = new Field
                {
                    FieldName = ViewLang.JobGrade,
                    LinkCreateItem = DataConfigurationActionLink.CreateJobGrade,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateJobGrade
                },
                IsHost = sessionService.HasPermissionOnFunction(FunctionKey.DataConfiguration, Policy.Host)
            };

            return PartialView("_JobGradeTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateJobGrade()
        {
            var viewModel = new CreateJobGradeViewModel
            {
                JobCategoryList = await dataConfigurationService.GetJobCategorySelectListAsync(),
                Field = new Field
                {
                    FieldName = ViewLang.JobGrade,
                    LinkCreateItem = DataConfigurationActionLink.CreateJobGrade
                }
            };

            return PartialView("_CreateJobGrade", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateJobGrade(JobGrade jobGrade)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateJobGradeAsync(jobGrade);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = string.Format(Message.ItemCreatedSuccessful, jobGrade.Name) });
                }
                else
                {
                    var modelName = string.Concat(nameof(jobGrade), ".", nameof(jobGrade.Name));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
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
                    JobGrade = jobGrade,
                    Field = new Field
                    {
                        FieldName = ViewLang.JobGrade,
                        LinkUpdateItem = DataConfigurationActionLink.UpdateJobGrade,
                    }
                };

                return PartialView("_UpdateJobGrade", viewModel);
            }

            return Content(Message.NotFoundError);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateJobGrade(JobGrade jobGrade)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.UpdateJobGradeAsync(jobGrade);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = Message.ItemUpdatedSuccessful });
                }
                else
                {
                    var modelName = string.Concat(nameof(jobGrade), ".", nameof(jobGrade.Name));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        #endregion Job Grade

        #region Country

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Read)]
        public async Task<IActionResult> GetAllCountry()
        {
            var countries = await dataConfigurationService.GetAllCountryAsync();
            var viewModel = new SimpleItemTableViewModel
            {
                Items = countries,
                Field = new Field
                {
                    FieldName = ViewLang.Country,
                    LinkCreateItem = DataConfigurationActionLink.CreateCountry,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateCountry
                },
                IsHost = sessionService.HasPermissionOnFunction(FunctionKey.DataConfiguration, Policy.Host)
            };

            return PartialView("_SimpleItemTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public IActionResult CreateCountry()
        {
            var viewModel = new CreateSimpleItemViewModel
            {
                Field = new Field
                {
                    FieldName = ViewLang.Country,
                    LinkCreateItem = DataConfigurationActionLink.CreateCountry
                }
            };

            return PartialView("_CreateSimpleItem", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateCountry(SimpleItem item)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateCountryAsync(item);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = string.Format(Message.ItemCreatedSuccessful, item.ItemName) });
                }
                else
                {
                    var modelName = string.Concat(nameof(item), ".", nameof(item.ItemName));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateCountry(int id)
        {
            var country = await dataConfigurationService.GetCountryByIdAsync(id);

            if (country != null)
            {
                var viewModel = new UpdateSimpleItemViewModel
                {
                    Item = country,
                    Field = new Field
                    {
                        FieldName = ViewLang.Country,
                        LinkUpdateItem = DataConfigurationActionLink.UpdateCountry
                    }
                };

                return PartialView("_UpdateSimpleItem", viewModel);
            }

            return Content(Message.NotFoundError);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateCountry(SimpleItem item)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.UpdateCountryAsync(item);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = Message.ItemUpdatedSuccessful });
                }
                else
                {
                    var modelName = string.Concat(nameof(item), ".", nameof(item.ItemName));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        #endregion Country

        #region City

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Read)]
        public async Task<IActionResult> GetAllCity()
        {
            var cities = await dataConfigurationService.GetAllCityAsync();
            var viewModel = new CityTableViewModel
            {
                Cities = cities,
                Field = new Field
                {
                    FieldName = ViewLang.City,
                    LinkCreateItem = DataConfigurationActionLink.CreateCity,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateCity
                },
                IsHost = sessionService.HasPermissionOnFunction(FunctionKey.DataConfiguration, Policy.Host)
            };

            return PartialView("_CityTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateCity()
        {
            var viewModel = new CreateCityViewModel
            {
                CountryList = await dataConfigurationService.GetCountrySelectListAsync(),
                Field = new Field
                {
                    FieldName = ViewLang.City,
                    LinkCreateItem = DataConfigurationActionLink.CreateCity
                }
            };

            return PartialView("_CreateCity", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateCity(City city)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateCityAsync(city);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = string.Format(Message.ItemCreatedSuccessful, city.Code) });
                }
                else
                {
                    var modelName = string.Concat(nameof(city), ".", nameof(city.Code));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
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
                    City = city,
                    Field = new Field
                    {
                        FieldName = ViewLang.City,
                        LinkUpdateItem = DataConfigurationActionLink.UpdateCity,
                    }
                };

                return PartialView("_UpdateCity", viewModel);
            }

            return Content(Message.NotFoundError);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateCity(City city)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.UpdateCityAsync(city);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = Message.ItemUpdatedSuccessful });
                }
                else
                {
                    var modelName = string.Concat(nameof(city), ".", nameof(city.Code));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
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
                Field = new Field
                {
                    FieldName = ViewLang.Office,
                    LinkCreateItem = DataConfigurationActionLink.CreateOffice,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateOffice
                },
                IsHost = sessionService.HasPermissionOnFunction(FunctionKey.DataConfiguration, Policy.Host)
            };

            return PartialView("_OfficeTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateOffice()
        {
            var viewModel = new CreateOfficeViewModel
            {
                CountryList = await dataConfigurationService.GetCountrySelectListAsync(),
                Field = new Field
                {
                    FieldName = ViewLang.Office,
                    LinkCreateItem = DataConfigurationActionLink.CreateOffice
                }
            };

            return PartialView("_CreateOffice", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateOffice(Office office)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateOfficeAsync(office);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = string.Format(Message.ItemCreatedSuccessful, office.Code) });
                }
                else
                {
                    var modelName = string.Concat(nameof(office), ".", nameof(office.Code));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
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
                    Office = office,
                    Field = new Field
                    {
                        FieldName = ViewLang.Office,
                        LinkUpdateItem = DataConfigurationActionLink.UpdateOffice,
                    }
                };

                return PartialView("_UpdateOffice", viewModel);
            }

            return Content(Message.NotFoundError);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateOffice(Office office)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.UpdateOfficeAsync(office);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = Message.ItemUpdatedSuccessful });
                }
                else
                {
                    var modelName = string.Concat(nameof(office), ".", nameof(office.Code));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        #endregion Office

        #region RefResource Item

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Read)]
        public async Task<IActionResult> GetAllRefResourceItem(int fieldId)
        {
            var field = FieldsDictionary.Fields[fieldId];
            var items = await dataConfigurationService.GetAllRefResourceItemAsync(field.RefName);
            var viewModel = new SimpleItemTableViewModel
            {
                Items = items,
                Field = field,
                IsHost = sessionService.HasPermissionOnFunction(FunctionKey.DataConfiguration, Policy.Host)
            };

            return PartialView("_SimpleItemTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public IActionResult CreateRefResourceItem(int fieldId)
        {
            var field = FieldsDictionary.Fields[fieldId];
            var viewModel = new CreateSimpleItemViewModel
            {
                Field = field
            };

            return PartialView("_CreateSimpleItem", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateRefResourceItem(int fieldId, SimpleItem item)
        {
            var field = FieldsDictionary.Fields[fieldId];

            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateRefResourceItemAsync(field.RefName, item);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = string.Format(Message.ItemCreatedSuccessful, item.ItemName) });
                }
                else
                {
                    var modelName = string.Concat(nameof(item), ".", nameof(item.ItemName));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateRefResourceItem(int fieldId, int id)
        {
            var field = FieldsDictionary.Fields[fieldId];
            var item = await dataConfigurationService.GetRefResourceByNameAndIdAsync(field.RefName, id);

            if (item != null)
            {
                var viewModel = new UpdateSimpleItemViewModel
                {
                    Item = item,
                    Field = field
                };

                return PartialView("_UpdateSimpleItem", viewModel);
            }

            return Content(Message.NotFoundError);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateRefResourceItem(int fieldId, SimpleItem item)
        {
            var field = FieldsDictionary.Fields[fieldId];

            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.UpdateRefResourceItemAsync(field.RefName, item);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = Message.ItemUpdatedSuccessful });
                }
                else
                {
                    var modelName = string.Concat(nameof(item), ".", nameof(item.ItemName));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        #endregion RefResource Item

        #region Currency

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Read)]
        public async Task<IActionResult> GetAllCurrency()
        {
            var currencies = await dataConfigurationService.GetAllCurrencyAsync();
            var viewModel = new SimpleItemTableViewModel
            {
                Items = currencies,
                Field = new Field
                {
                    FieldName = ViewLang.Currency,
                    LinkCreateItem = DataConfigurationActionLink.CreateCurrency,
                    LinkUpdateItem = DataConfigurationActionLink.UpdateCurrency
                },
                IsHost = sessionService.HasPermissionOnFunction(FunctionKey.DataConfiguration, Policy.Host)
            };

            return PartialView("_SimpleItemTable", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public IActionResult CreateCurrency()
        {
            var viewModel = new CreateSimpleItemViewModel
            {
                Field = new Field
                {
                    FieldName = ViewLang.Currency,
                    LinkCreateItem = DataConfigurationActionLink.CreateCurrency
                }
            };

            return PartialView("_CreateSimpleItem", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> CreateCurrency(SimpleItem item)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.CreateCurrencyAsync(item);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = string.Format(Message.ItemCreatedSuccessful, item.ItemName) });
                }
                else
                {
                    var modelName = string.Concat(nameof(item), ".", nameof(item.ItemName));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateCurrency(int id)
        {
            var currency = await dataConfigurationService.GetCurrencyByIdAsync(id);

            if (currency != null)
            {
                var viewModel = new UpdateSimpleItemViewModel
                {
                    Item = currency,
                    Field = new Field
                    {
                        FieldName = ViewLang.Currency,
                        LinkUpdateItem = DataConfigurationActionLink.UpdateCurrency
                    }
                };

                return PartialView("_UpdateSimpleItem", viewModel);
            }

            return Content(Message.NotFoundError);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateCurrency(SimpleItem item)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await dataConfigurationService.UpdateCurrencyAsync(item);
                if (commandResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = commandResult.IsSuccessful, message = Message.ItemUpdatedSuccessful });
                }
                else
                {
                    var modelName = string.Concat(nameof(item), ".", nameof(item.ItemName));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        #endregion Currency

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