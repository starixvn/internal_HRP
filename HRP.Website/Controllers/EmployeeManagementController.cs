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
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models;
    using Models.EmployeeManagement;
    using Services;

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
        public IActionResult Index(
            string departmentIds = null,
            string employeeStatusIds = null,
            int? employeeId = null,
            string displayName = null)
        {
            var listDepartment = employeeManagementService.GetAllCompanyDivisonDepartment();
            var listEmployeeStatus = baseService.GetData<ReferenceResourceModel>(new GetRefResourceByNamesCriteria { RefNames = ReferenceResource.EmploymentStatus });
            var viewModel = new EmployeeManagementViewModel
            {
                IsHostCreateEmployee = sessionService.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host),
                DepartmentIds = departmentIds ?? GetDepartmentIds(listDepartment.Departments),
                EmployeeStatusIds = employeeStatusIds ?? string.Join(",", listEmployeeStatus.Select(x => x.ResourceId).ToList()),
                EmployeeStatus = new SelectList(listEmployeeStatus, "ResourceId", "Value"),
                EmployeeId = employeeId,
                DisplayName = displayName
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

        [HRPPermission(FunctionKey = FunctionKey.EmployeeManagement, Policy = Policy.Read)]
        public IActionResult EmployeeList(EmployeeManagementViewModel model)
        {
            var criteria = new GetAllEmployeeCriteria
            {
                DepartmentIds = model.DepartmentIds,
                EmployeeStatusIds = model.EmployeeStatusIds,
                EmployeeId = model.EmployeeId,
                UserId = sessionService.UserId
            };
            var employees = baseService.GetData<EmployeeBasicInfo>(criteria);
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
        public async Task<IActionResult> CreateEmployee(
            string departmentIds = null,
            string employeeStatusIds = null,
            int? employeeId = null)
        {
            var viewModel = await employeeManagementService.InitDataForNewEmployeeAsync();
            viewModel.Filter.DepartmentIds = departmentIds;
            viewModel.Filter.EmployeeStatusIds = employeeStatusIds;
            viewModel.Filter.EmployeeId = employeeId;

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
        public IActionResult CreateEmployment(
            string departmentIds = null,
            string employeeStatusIds = null,
            int? employeeId = null)
        {
            CreateEmploymentViewModel viewModel = employeeManagementService.GetDataToCreateEmployment();
            viewModel.Filter.DepartmentIds = departmentIds;
            viewModel.Filter.EmployeeStatusIds = employeeStatusIds;
            viewModel.Filter.EmployeeId = employeeId;

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
        public IActionResult UpdateEmployee(
            int id,
            string departmentIds = null,
            string employeeStatusIds = null,
            int? employeeId = null)
        {
            var viewModel = employeeManagementService.GetEmployeeById(id);

            if (viewModel == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            viewModel.Filter.DepartmentIds = departmentIds;
            viewModel.Filter.EmployeeStatusIds = employeeStatusIds;
            viewModel.Filter.EmployeeId = employeeId;

            return View("UpdateEmployee", viewModel);
        }

        [Route("EmployeeManagement/Update/PersonalDetails")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> UpdatePersonalDetails(CreateEmployeeViewModel employee)
        {
            var validatePicture = employeeManagementService.CheckValidPicture(employee.ProfilePicture);
            if (!validatePicture.IsSuccessful)
            {
                ModelState.AddModelError(nameof(employee.ProfilePicture), validatePicture.Message);
            }

            if (ModelState.IsValid)
            {
                var commandResult = await employeeManagementService.UpdatePersonalDetailsAsync(employee);
                var returnMessage = commandResult.IsSuccessful ? string.Format(Message.UpdateEmployeeSuccessful, employee.Nickname) : commandResult.Message;

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = returnMessage });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        [Route("EmployeeManagement/Update/Employment")]
        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Read)]
        public IActionResult UpdateEmployment(
            int employeeId,
            string departmentIds = null,
            string employeeStatusIds = null,
            int? employeeFilterId = null)
        {
            UpdateEmploymentViewModel viewModel = employeeManagementService.GetDataToUpdateEmployment(employeeId);

            if (viewModel == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            viewModel.Filter.DepartmentIds = departmentIds;
            viewModel.Filter.EmployeeStatusIds = employeeStatusIds;
            viewModel.Filter.EmployeeId = employeeFilterId;

            return PartialView("_UpdateEmployment", viewModel);
        }

        [Route("EmployeeManagement/Update/Employment")]
        [HttpPost]
        [ValidateAntiForgeryToken]
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
            var criteria = new GetDivisionByCompanyIdCriteria { CompanyId = companyId, IsActive = true };
            var divisions = await baseService.GetDataAsync<Division>(criteria);

            return Json(divisions);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> GetDepartmentByDivisionId(int divisionId)
        {
            var criteria = new GetDepartmentByDivisionIdCriteria { DivisionId = divisionId, IsActive = true };
            var departments = await baseService.GetDataAsync<Department>(criteria);

            return Json(departments);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> GetCostCenterDepartmentId(int departmentId)
        {
            var criteria = new GetCostCenterDepartmentIdCriteria { DepartmentId = departmentId, IsActive = true };
            var costCenters = await baseService.GetDataAsync<CostCenter>(criteria);

            return Json(costCenters);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> GetSectionByCostCenterId(int costCenterId)
        {
            var criteria = new GetSectionByCostCenterIdCriteria { CostCenterId = costCenterId, IsActive = true };
            var sections = await baseService.GetDataAsync<Section>(criteria);

            return Json(sections);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> GetEmployeeSuggestion(string idNo)
        {
            var criteria = new GetEmployeeSuggestionByIDNoCriteria { IDNo = idNo };
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
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> GetCityByCountryId(int countryId)
        {
            var criteria = new GetCityByCountryIdCriteria { CountryId = countryId, IsActive = true };
            var sections = await baseService.GetDataAsync<City>(criteria);

            return Json(sections);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> GetOfficeByCityId(int cityId)
        {
            var criteria = new GetOfficeByCityIdCriteria { CityId = cityId, IsActive = true };
            var sections = await baseService.GetDataAsync<Office>(criteria);

            return Json(sections);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeePayroll, Policy = Policy.Read)]
        public IActionResult Payroll(int id)
        {
            var viewModel = payrollService.GetEmployeePayroll(id, sessionService.UserId);

            if (viewModel == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeePayroll, Policy = Policy.Host)]
        public IActionResult SavePayroll(EmployeePayrollViewModel model)
        {
            if (ModelState.IsValid)
            {
                payrollService.UpdatePayroll(model, sessionService.UserId);
                return RedirectToAction("Index");
            }

            return View("Payroll", model);
        }
    }
}