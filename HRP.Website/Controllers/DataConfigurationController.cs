namespace HRP.Website.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Filters;
    using HRP.Core.Services;
    using HRP.Models;
    using HRP.Models.DataConfiguration;
    using HRP.Models.DataConfiguration.CostCenter;
    using HRP.Models.DataConfiguration.Department;
    using HRP.Models.DataConfiguration.Division;
    using HRP.Models.DataConfiguration.GradeLevel;
    using HRP.Models.DataConfiguration.JobGrade;
    using HRP.Models.DataConfiguration.Section;
    using HRP.Resources;
    using HRP.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class DataConfigurationController : BaseController
    {
        private readonly ICommonService commonService;
        private readonly IDataConfigurationService dataConfigurationService;

        public DataConfigurationController(
            ISessionService sessionService,
            IBaseService baseService,
            ICommonService commonService,
            IDataConfigurationService dataConfigurationService) : base(baseService, sessionService)
        {
            this.commonService = commonService;
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
                    return JsonResultWithCreatedMessage(item.ItemName);
                }
                else
                {
                    var modelName = string.Concat(nameof(item), ".", nameof(item.ItemName));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            return JsonResultWithModelStateErrors(ModelState);
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
                    return JsonResultWithUpdatedMessage();
                }
                else
                {
                    var modelName = string.Concat(nameof(item), ".", nameof(item.ItemName));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            return JsonResultWithModelStateErrors(ModelState);
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
                    return JsonResultWithCreatedMessage(division.Code);
                }
                else
                {
                    var modelName = string.Concat(nameof(division), ".", nameof(division.Code));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            return JsonResultWithModelStateErrors(ModelState);
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
                    return JsonResultWithUpdatedMessage();
                }
                else
                {
                    var modelName = string.Concat(nameof(division), ".", nameof(division.Code));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            return JsonResultWithModelStateErrors(ModelState);
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
                    return JsonResultWithCreatedMessage(department.Code);
                }
                else
                {
                    var modelName = string.Concat(nameof(department), ".", nameof(department.Code));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            return JsonResultWithModelStateErrors(ModelState);
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
                    return JsonResultWithUpdatedMessage();
                }
                else
                {
                    var modelName = string.Concat(nameof(department), ".", nameof(department.Code));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            return JsonResultWithModelStateErrors(ModelState);
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
                    return JsonResultWithCreatedMessage(costCenter.Code);
                }
                else
                {
                    var modelName = string.Concat(nameof(costCenter), ".", nameof(costCenter.Code));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            return JsonResultWithModelStateErrors(ModelState);
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
                    return JsonResultWithUpdatedMessage();
                }
                else
                {
                    var modelName = string.Concat(nameof(costCenter), ".", nameof(costCenter.Code));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            return JsonResultWithModelStateErrors(ModelState);
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
                    return JsonResultWithCreatedMessage(section.Code);
                }
                else
                {
                    var modelName = string.Concat(nameof(section), ".", nameof(section.Code));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            return JsonResultWithModelStateErrors(ModelState);
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
                    return JsonResultWithUpdatedMessage();
                }
                else
                {
                    var modelName = string.Concat(nameof(section), ".", nameof(section.Code));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            return JsonResultWithModelStateErrors(ModelState);
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
                    return JsonResultWithCreatedMessage(item.ItemName);
                }
                else
                {
                    var modelName = string.Concat(nameof(item), ".", nameof(item.ItemName));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            return JsonResultWithModelStateErrors(ModelState);
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
                    return JsonResultWithUpdatedMessage();
                }
                else
                {
                    var modelName = string.Concat(nameof(item), ".", nameof(item.ItemName));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            return JsonResultWithModelStateErrors(ModelState);
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
            IEnumerable<JobCategory> jobCategories = await commonService.GetAllJobCategoryAsync(null);
            var viewModel = new CreateGradeLevelViewModel
            {
                JobCategoryList = commonService.GetSelectList(jobCategories, nameof(JobCategory.JobCategoryId), nameof(JobCategory.Name)),
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
                    return JsonResultWithCreatedMessage(gradeLevel.Name);
                }
                else
                {
                    var modelName = string.Concat(nameof(gradeLevel), ".", nameof(gradeLevel.Name));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            return JsonResultWithModelStateErrors(ModelState);
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
                    return JsonResultWithUpdatedMessage();
                }
                else
                {
                    var modelName = string.Concat(nameof(gradeLevel), ".", nameof(gradeLevel.Name));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            return JsonResultWithModelStateErrors(ModelState);
        }

        #endregion Grade Level

        #region Job Grade

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Read)]
        public async Task<IActionResult> GetAllJobGrade()
        {
            var jobGradeList = await dataConfigurationService.GetAllJobGradeAsync();
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
            IEnumerable<JobCategory> jobCategories = await commonService.GetAllJobCategoryAsync(null);
            var viewModel = new CreateJobGradeViewModel
            {
                JobCategoryList = commonService.GetSelectList(jobCategories, nameof(JobCategory.JobCategoryId), nameof(JobCategory.Name)),
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
                    return JsonResultWithCreatedMessage(jobGrade.Name);
                }
                else
                {
                    var modelName = string.Concat(nameof(jobGrade), ".", nameof(jobGrade.Name));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            return JsonResultWithModelStateErrors(ModelState);
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
                    return JsonResultWithUpdatedMessage();
                }
                else
                {
                    var modelName = string.Concat(nameof(jobGrade), ".", nameof(jobGrade.Name));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            return JsonResultWithModelStateErrors(ModelState);
        }

        #endregion Job Grade

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
                    return JsonResultWithCreatedMessage(item.ItemName);
                }
                else
                {
                    var modelName = string.Concat(nameof(item), ".", nameof(item.ItemName));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            return JsonResultWithModelStateErrors(ModelState);
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
                    return JsonResultWithUpdatedMessage();
                }
                else
                {
                    var modelName = string.Concat(nameof(item), ".", nameof(item.ItemName));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            return JsonResultWithModelStateErrors(ModelState);
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
                    return JsonResultWithCreatedMessage(item.ItemName);
                }
                else
                {
                    var modelName = string.Concat(nameof(item), ".", nameof(item.ItemName));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            return JsonResultWithModelStateErrors(ModelState);
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
                    return JsonResultWithUpdatedMessage();
                }
                else
                {
                    var modelName = string.Concat(nameof(item), ".", nameof(item.ItemName));
                    ModelState.AddModelError(modelName, commandResult.Message);
                }
            }

            return JsonResultWithModelStateErrors(ModelState);
        }

        #endregion Currency

        #region Commons

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> GetDivisionSelectListByCompanyId(int companyId)
        {
            IEnumerable<Division> divisions = await commonService.GetDivisionByCompanyIdAsync(companyId, null);
            return PartialView("_SelectListItem", commonService.GetSelectList(divisions, nameof(Division.DivisionId), nameof(Division.Code)));
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> GetDepartmentSelectListByDivisionId(int divisionId)
        {
            IEnumerable<Department> departments = await commonService.GetDepartmentByDivisionIdAsync(divisionId, null);
            return PartialView("_SelectListItem", commonService.GetSelectList(departments, nameof(Department.DepartmentId), nameof(Department.Code)));
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> GetCostCenterSelectListByDepartmentId(int departmentId)
        {
            IEnumerable<CostCenter> costCenters = await commonService.GetCostCenterByDepartmentIdAsync(departmentId, null);
            return PartialView("_SelectListItem", commonService.GetSelectList(costCenters, nameof(CostCenter.CostCenterId), nameof(CostCenter.Code)));
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> GetSectionSelectListByCostCenterId(int costCenterId)
        {
            IEnumerable<Section> sections = await commonService.GetSectionByCostCenterIdAsync(costCenterId, null);
            return PartialView("_SelectListItem", commonService.GetSelectList(sections, nameof(Section.SectionId), nameof(Section.Code)));
        }

        [HRPPermission(FunctionKey = FunctionKey.DataConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> GetGradeLevelSelectListByJobCategoryId(int jobCategoryId)
        {
            IEnumerable<GradeLevel> gradeLevels = await commonService.GetGradeLevelByJobCategoryIdAsync(jobCategoryId, null);
            return PartialView("_SelectListItem", commonService.GetSelectList(gradeLevels, nameof(GradeLevel.GradeLevelId), nameof(GradeLevel.Name)));
        }

        private static string UppercaseFirstLetter(string s)
        {
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);

            return new string(a);
        }

        private IActionResult JsonResultWithCreatedMessage(string itemName)
        {
            return Json(new { isSuccessful = true, message = string.Format(Message.ItemCreatedSuccessful, itemName) });
        }

        private IActionResult JsonResultWithUpdatedMessage()
        {
            return Json(new { isSuccessful = true, message = Message.ItemUpdatedSuccessful });
        }

        private IActionResult JsonResultWithModelStateErrors(ModelStateDictionary modelState)
        {
            var errors = modelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => UppercaseFirstLetter(x.Key), x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        #endregion Commons
    }
}