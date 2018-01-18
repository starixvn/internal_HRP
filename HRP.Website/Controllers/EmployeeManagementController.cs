namespace HRP.Website.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Filters;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Criteria.Employee;
    using HRP.Resources;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.EmployeeManagement;
    using Services;
    using Services.Interfaces;

    [HRPAuthorize]
    public class EmployeeManagementController : Controller
    {
        private readonly IBaseService baseService;
        private readonly IEmployeeManagementService employeeManagementService;
        private readonly IEmployeePayrollService payrollService;
        private readonly ISessionService sessionService;

        public EmployeeManagementController(
            IBaseService baseService,
            IEmployeeManagementService employeeManagementService,
            IEmployeePayrollService payrollService,
            ISessionService sessionService)
        {
            this.baseService = baseService;
            this.employeeManagementService = employeeManagementService;
            this.payrollService = payrollService;
            this.sessionService = sessionService;
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeManagement, Policy = Policy.Read)]
        public IActionResult Index()
        {
            var listDepartment = employeeManagementService.GetAllCompanyDivisonDepartment();
            var viewModel = new EmployeeManagementViewModel
            {
                IsHostCreateEmployee = sessionService.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host),
                DepartmentIds = GetDepartmentIds(listDepartment.Departments),
                InService = Selection.Yes,
                InProbation = Selection.All,
            };
            return View(viewModel);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeManagement, Policy = Policy.Read)]
        public IActionResult GetCompanyTree()
        {
            var listDepartment = employeeManagementService.GetAllCompanyDivisonDepartment();
            return Json(listDepartment);
        }

        private static string GetDepartmentIds(IEnumerable<Department> departments)
        {
            var departmentIds = string.Empty;
            departmentIds = string.Join(",", departments.Select(x => x.DepartmentId).ToList());
            return departmentIds;
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeManagement, Policy = Policy.Read)]
        public IActionResult EmployeeList(EmployeeManagementViewModel model)
        {
            var criteria = new GetAllEmployeeCriteria
            {
                DepartmentIds = string.IsNullOrEmpty(model.DepartmentIds) ? string.Empty : model.DepartmentIds,
                InService = model.InService,
                InProbation = model.InProbation,
                EmployeeId = model.EmployeeId,
                UserId = sessionService.UserId
            };
            var employees = baseService.GetData<EmployeeList>(criteria);
            foreach (var employee in employees)
            {
                employee.IsHostPayroll = sessionService.HasPermissionOnFunction(FunctionKey.EmployeePayroll, Policy.Host);
                employee.IsHostProfile = sessionService.HasPermissionOnFunction(FunctionKey.EmployeeProfile, Policy.Host);
                employee.IsHostBankDetails = sessionService.HasPermissionOnFunction(FunctionKey.EmployeeBankDetails, Policy.Host);
            }
            var employeesSorted = employees.OrderBy(e => e.CompanyCode).ThenBy(e => e.IDNo).ToList();
            return PartialView("_ListEmployee", employeesSorted);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeManagement, Policy = Policy.Read)]
        public async Task<IActionResult> GetEmployeeSuggestionByName(string name)
        {
            var criteria = new GetEmployeeSuggestionByNameCriteria
            {
                Name = name,
                UserId = sessionService.UserId
            };
            var employees = await baseService.GetDataAsync<EmployeeSuggestionViewModel>(criteria);

            return Json(employees);
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> CreateEmployee()
        {
            var viewModel = await employeeManagementService.InitDataForNewEmployeeAsync();

            return View("CreateEmployee", viewModel);
        }

        [Route("EmployeeManagement/Create/PersonalDetails")]
        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public IActionResult CreatePersonalDetails(CreateEmployeeViewModel employee)
        {
            var validatePicture = employeeManagementService.CheckValidPicture(employee.ProfilePicture);
            if (!validatePicture.IsSuccessful)
            {
                ModelState.AddModelError(nameof(employee.ProfilePicture), validatePicture.Message);
            }

            if (ModelState.IsValid)
            {
                return Json(new { isSuccessful = true });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);
            return Json(new { isSuccessful = false, errors });
        }

        [Route("EmployeeManagement/Create/Employment")]
        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> CreateEmployment()
        {
            var viewModel = await InitDataForCreateEmployment();

            return PartialView("_CreateEmployment", viewModel);
        }

        [Route("EmployeeManagement/Create/Employment")]
        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> CreateEmployment(CreateEmploymentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var commandResult = await employeeManagementService.CreateEmployeeAsync(viewModel);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);
            return Json(new { isSuccessful = false, errors });
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Read)]
        public IActionResult EditEmployee(int id, EmployeeTab tab = EmployeeTab.PersonalDetail)
        {
            var viewModel = employeeManagementService.GetEmployeeById(id);
            ViewBag.ActivePanel = tab.ToString();

            if (viewModel == null)
            {
                return RedirectToAction("Index");
            }

            viewModel.IsEditForm = true;

            return View("EditEmployee", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> EditEmployee(CreateEmployeeViewModel employee)
        {
            var validatePicture = employeeManagementService.CheckValidPicture(employee.ProfilePicture);
            if (!validatePicture.IsSuccessful)
            {
                ModelState.AddModelError(nameof(employee.ProfilePicture), validatePicture.Message);
            }

            if (ModelState.IsValid)
            {
                var commandResult = await employeeManagementService.EditEmployeeAsync(employee);
                var returnMessage = commandResult.IsSuccessful ? string.Format(Message.UpdateEmployeeSuccessful, employee.NickName) : commandResult.Message;

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = returnMessage });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Read)]
        public async Task<IActionResult> UpdateEmployment(int employeeId)
        {
            var refNames = string.Format(
                "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                ReferenceResource.LocalCompany,
                ReferenceResource.LocalJobTitle,
                ReferenceResource.EmploymentType,
                ReferenceResource.EmploymentStatus,
                ReferenceResource.Country,
                ReferenceResource.City,
                ReferenceResource.Office,
                ReferenceResource.WorkPermitType1,
                ReferenceResource.WorkPermitType2,
                ReferenceResource.CommonReasons,
                ReferenceResource.GroupDateReasons);
            var refResources = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNamesCriteria { RefNames = refNames });

            UpdateEmploymentViewModel viewModel = employeeManagementService.GetEmploymentToUpdate(employeeId);
            viewModel.Companies = await baseService.GetDataAsync<Company>(new GetAllCompanyCriteria { IsActive = true });
            viewModel.LocalCompanies = GetRefResourceByName(refResources, ReferenceResource.LocalCompany);
            viewModel.LocalJobTitles = GetRefResourceByName(refResources, ReferenceResource.LocalJobTitle);
            viewModel.EmploymentTypes = GetRefResourceByName(refResources, ReferenceResource.EmploymentType);
            viewModel.EmploymentStatuses = GetRefResourceByName(refResources, ReferenceResource.EmploymentStatus);
            viewModel.Countries = GetRefResourceByName(refResources, ReferenceResource.Country);
            viewModel.Cities = GetRefResourceByName(refResources, ReferenceResource.City);
            viewModel.Offices = GetRefResourceByName(refResources, ReferenceResource.Office);
            viewModel.WorkPermitType1s = GetRefResourceByName(refResources, ReferenceResource.WorkPermitType1);
            viewModel.WorkPermitType2s = GetRefResourceByName(refResources, ReferenceResource.WorkPermitType2);
            viewModel.CommonReasons = GetRefResourceByName(refResources, ReferenceResource.CommonReasons);
            viewModel.GroupDateReasons = GetRefResourceByName(refResources, ReferenceResource.GroupDateReasons);
            viewModel.JobCategories = await baseService.GetDataAsync<JobCategory>(new GetAllJobCategoryCriteria { IsActive = true });
            viewModel.GradeLevels = await baseService.GetDataAsync<GradeLevel>(new GetGradeLevelByJobCategoryIdCriteria
            {
                JobCategoryId = viewModel.JobCategoryId,
                IsActive = true
            });
            viewModel.JobGrades = await baseService.GetDataAsync<JobGrade>(new GetJobGradeByGradeLevelIdCriteria
            {
                GradeLevelId = viewModel.GradeLevelId,
                IsActive = true
            });
            viewModel.Divisions = await baseService.GetDataAsync<Division>(new GetDivisionByCompanyIdCriteria { CompanyId = viewModel.CompanyId });
            viewModel.Departments = await baseService.GetDataAsync<Department>(new GetDepartmentByDivisionIdCriteria { DivisionId = viewModel.DivisionId });
            viewModel.Sections = await baseService.GetDataAsync<Section>(new GetSectionByDepartmentIdCriteria { DepartmentId = viewModel.DepartmentId });
            viewModel.CostCenters = await baseService.GetDataAsync<CostCenter>(new GetCostCenterBySectionIdCriteria { SectionId = viewModel.SectionId });

            return PartialView("_UpdateEmployment", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateEmployment(UpdateEmploymentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await employeeManagementService.UpdateEmploymentAsync(viewModel);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);
            return Json(new { isSuccessful = false, errors });
        }

        public async Task<IActionResult> CheckEmployeeCodeExisted(int employeeId, string companyCode, string employeeCode)
        {
            var criteria = new CheckEmpCodeExistedCriteria
            {
                EmployeeId = employeeId,
                CompanyCode = companyCode,
                EmployeeCode = employeeCode
            };

            var existed = await baseService.GetDataEntityAsync<bool>(criteria);

            return Json(!existed);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> GetDivisionByCompanyId(int companyId)
        {
            var criteria = new GetDivisionByCompanyIdCriteria { CompanyId = companyId };
            var divisions = await baseService.GetDataAsync<Division>(criteria);

            return Json(divisions);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> GetDepartmentByDivisionId(int divisionId)
        {
            var criteria = new GetDepartmentByDivisionIdCriteria { DivisionId = divisionId };
            var departments = await baseService.GetDataAsync<Department>(criteria);

            return Json(departments);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> GetSectionByDepartmentId(int departmentId)
        {
            var criteria = new GetSectionByDepartmentIdCriteria { DepartmentId = departmentId };
            var sections = await baseService.GetDataAsync<Section>(criteria);

            return Json(sections);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> GetCostCenterBySectionId(int sectionId)
        {
            var criteria = new GetCostCenterBySectionIdCriteria { SectionId = sectionId };
            var costCenters = await baseService.GetDataAsync<CostCenter>(criteria);

            return Json(costCenters);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> GetEmployeeSuggestion(string idNo)
        {
            var criteria = new GetEmployeeSuggestionCriteria { IDNo = idNo };
            var employees = await baseService.GetDataAsync<EmployeeSuggestionViewModel>(criteria);

            return Json(employees);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> GetGradeLevelByJobCategoryId(int jobCategoryId)
        {
            var criteria = new GetGradeLevelByJobCategoryIdCriteria { JobCategoryId = jobCategoryId };
            var gradeLevels = await baseService.GetDataAsync<GradeLevel>(criteria);

            return Json(gradeLevels);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> GetJobGradeByGradeLevelId(int gradeLevelId)
        {
            var criteria = new GetJobGradeByGradeLevelIdCriteria { GradeLevelId = gradeLevelId };
            var jobGrades = await baseService.GetDataAsync<JobGrade>(criteria);

            return Json(jobGrades);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeePayroll, Policy = Policy.Read)]
        public IActionResult Payroll(int id)
        {
            var viewModel = payrollService.GetEmployeePayroll(id);
            return View(viewModel);
        }

        private static IEnumerable<ReferenceResourceModel> GetRefResourceByName(IEnumerable<ReferenceResourceModel> refResources, string refName)
        {
            return refResources.Where(x => x.RefName == refName);
        }

        private async Task<CreateEmploymentViewModel> InitDataForCreateEmployment()
        {
            var refNames = string.Format(
                "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                ReferenceResource.LocalCompany,
                ReferenceResource.LocalJobTitle,
                ReferenceResource.EmploymentType,
                ReferenceResource.EmploymentStatus,
                ReferenceResource.Country,
                ReferenceResource.City,
                ReferenceResource.Office,
                ReferenceResource.WorkPermitType1,
                ReferenceResource.WorkPermitType2,
                ReferenceResource.CommonReasons,
                ReferenceResource.GroupDateReasons);
            var refResources = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNamesCriteria { RefNames = refNames });

            var viewModel = new CreateEmploymentViewModel
            {
                LocalCompanies = GetRefResourceByName(refResources, ReferenceResource.LocalCompany),
                LocalJobTitles = GetRefResourceByName(refResources, ReferenceResource.LocalJobTitle),
                EmploymentTypes = GetRefResourceByName(refResources, ReferenceResource.EmploymentType),
                EmploymentStatuses = GetRefResourceByName(refResources, ReferenceResource.EmploymentStatus),
                Countries = GetRefResourceByName(refResources, ReferenceResource.Country),
                Cities = GetRefResourceByName(refResources, ReferenceResource.City),
                Offices = GetRefResourceByName(refResources, ReferenceResource.Office),
                WorkPermitType1s = GetRefResourceByName(refResources, ReferenceResource.WorkPermitType1),
                WorkPermitType2s = GetRefResourceByName(refResources, ReferenceResource.WorkPermitType2),
                CommonReasons = GetRefResourceByName(refResources, ReferenceResource.CommonReasons),
                GroupDateReasons = GetRefResourceByName(refResources, ReferenceResource.GroupDateReasons)
            };
            viewModel.JobCategories = await baseService.GetDataAsync<JobCategory>(new GetAllJobCategoryCriteria { IsActive = true });
            viewModel.GradeLevels = await baseService.GetDataAsync<GradeLevel>(new GetGradeLevelByJobCategoryIdCriteria
            {
                JobCategoryId = viewModel.JobCategoryId,
                IsActive = true
            });
            viewModel.JobGrades = await baseService.GetDataAsync<JobGrade>(new GetJobGradeByGradeLevelIdCriteria
            {
                GradeLevelId = viewModel.GradeLevelId,
                IsActive = true
            });
            viewModel.Companies = await baseService.GetDataAsync<Company>(new GetAllCompanyCriteria { IsActive = true });
            viewModel.Divisions = await baseService.GetDataAsync<Division>(new GetDivisionByCompanyIdCriteria { CompanyId = viewModel.CompanyId });
            viewModel.Departments = await baseService.GetDataAsync<Department>(new GetDepartmentByDivisionIdCriteria { DivisionId = viewModel.DivisionId });
            viewModel.Sections = await baseService.GetDataAsync<Section>(new GetSectionByDepartmentIdCriteria { DepartmentId = viewModel.DepartmentId });
            viewModel.CostCenters = await baseService.GetDataAsync<CostCenter>(new GetCostCenterBySectionIdCriteria { SectionId = viewModel.SectionId });

            return viewModel;
        }
    }
}