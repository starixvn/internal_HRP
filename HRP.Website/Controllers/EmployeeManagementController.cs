namespace HRP.Website.Controllers
{
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Filters;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.Employee;
    using HRP.Models;
    using HRP.Models.EmployeeManagement;
    using HRP.Services;
    using Microsoft.AspNetCore.Mvc;

    [HRPAuthorize]
    public class EmployeeManagementController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IBaseService baseService;
        private readonly IEmployeeManagementService employeeManagementService;

        public EmployeeManagementController(
            ISessionService sessionService,
            IBaseService baseService,
            IEmployeeManagementService employeeManagementService)
        {
            this.sessionService = sessionService;
            this.baseService = baseService;
            this.employeeManagementService = employeeManagementService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public IActionResult CreateEmployee()
        {
            var model = new EmployeePersonalDetail();
            return View("PersonalDetail", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> CreateEmployee(EmployeePersonalDetail employee)
        {
            var validatePicture = employeeManagementService.CheckValidPicture(employee.ProfilePicture);
            if (!validatePicture.IsSuccessful)
            {
                ModelState.AddModelError(nameof(employee.ProfilePicture), validatePicture.Message);
            }

            if (ModelState.IsValid)
            {
                await employeeManagementService.CreateEmployeeAsync(employee);

                return RedirectToAction("Index");
            }

            return View("PersonalDetail", employee);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> CreateEmployment(int employeeId)
        {
            var bviCompanies = await baseService.GetDataAsync<Company>(new GetAllCompanyCriteria { Type = CompanyType.PVI, IsActive = true });
            var localCompanies = await baseService.GetDataAsync<Company>(new GetAllCompanyCriteria { Type = CompanyType.Local, IsActive = true });
            var localJobTitles = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNameCriteria { RefName = ReferenceResource.LocalJobTitle });
            var employmentTypes = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNameCriteria { RefName = ReferenceResource.EmploymentType });
            var employmentStatuses = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNameCriteria { RefName = ReferenceResource.EmploymentStatus });
            var jobCategories = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNameCriteria { RefName = ReferenceResource.JobCategory });
            var grades = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNameCriteria { RefName = ReferenceResource.Grade });
            var gradeLevels = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNameCriteria { RefName = ReferenceResource.GradeLevel });
            var countries = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNameCriteria { RefName = ReferenceResource.Country });
            var cities = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNameCriteria { RefName = ReferenceResource.City });
            var offices = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNameCriteria { RefName = ReferenceResource.Office });
            var taxDeclarations = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNameCriteria { RefName = ReferenceResource.TaxDeclaration });
            var workPermitType1s = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNameCriteria { RefName = ReferenceResource.WorkPermitType1 });
            var workPermitType2s = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNameCriteria { RefName = ReferenceResource.WorkPermitType2 });
            var viewModel = new CreateEmploymentViewModel
            {
                EmployeeId = employeeId,
                BVICompanies = bviCompanies,
                LocalCompanies = localCompanies,
                LocalJobTitles = localJobTitles,
                EmploymentTypes = employmentTypes,
                EmploymentStatuses = employmentStatuses,
                JobCategories = jobCategories,
                Grades = grades,
                GradeLevels = gradeLevels,
                Countries = countries,
                Cities = cities,
                Offices = offices,
                TaxDeclarations = taxDeclarations,
                WorkPermitType1s = workPermitType1s,
                WorkPermitType2s = workPermitType2s
            };

            return PartialView("_CreateEmployment", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public IActionResult CreateEmployment(CreateEmploymentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                return Json(0);
            }

            return Json(1);
        }

        public async Task<IActionResult> CheckEmployeeCodeExisted(string companyCode, string employeeCode)
        {
            var criteria = new CheckEmpCodeExistedCriteria
            {
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
    }
}