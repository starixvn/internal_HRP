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
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.EmployeeManagement;
    using Services;

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
            var viewModel = employeeManagementService.GetEmployeeById(0);

            return View("PersonalDetail", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeViewModel employee)
        {
            var validatePicture = employeeManagementService.CheckValidPicture(employee.ProfilePicture);
            if (!validatePicture.IsSuccessful)
            {
                ModelState.AddModelError(nameof(employee.ProfilePicture), validatePicture.Message);
            }

            if (ModelState.IsValid)
            {
                await employeeManagementService.CreateEmployeeAsync(employee);

                return Json(new { isSuccessful = true, message = "Successful!!!!!" });
            }

            var errorList = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
            );

            return Json(new { isSuccessful = false, errors = errorList });
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> CreateEmployment(int employeeId)
        {
            var refNames = string.Format(
                "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
                ReferenceResource.LocalCompany,
                ReferenceResource.LocalJobTitle,
                ReferenceResource.EmploymentType,
                ReferenceResource.EmploymentStatus,
                ReferenceResource.JobCategory,
                ReferenceResource.Grade,
                ReferenceResource.GradeLevel,
                ReferenceResource.Country,
                ReferenceResource.City,
                ReferenceResource.Office,
                ReferenceResource.TaxDeclaration,
                ReferenceResource.WorkPermitType1,
                ReferenceResource.WorkPermitType2);
            var bviCompanies = await baseService.GetDataAsync<Company>(new GetAllCompanyCriteria { IsActive = true });
            var refResources = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNamesCriteria { RefNames = refNames });
            var viewModel = new CreateEmploymentViewModel
            {
                EmployeeId = employeeId,
                Companies = bviCompanies,
                LocalCompanies = GetRefResourceByName(refResources, ReferenceResource.LocalCompany),
                LocalJobTitles = GetRefResourceByName(refResources, ReferenceResource.LocalJobTitle),
                EmploymentTypes = GetRefResourceByName(refResources, ReferenceResource.EmploymentType),
                EmploymentStatuses = GetRefResourceByName(refResources, ReferenceResource.EmploymentStatus),
                JobCategories = GetRefResourceByName(refResources, ReferenceResource.JobCategory),
                Grades = GetRefResourceByName(refResources, ReferenceResource.Grade),
                GradeLevels = GetRefResourceByName(refResources, ReferenceResource.GradeLevel),
                Countries = GetRefResourceByName(refResources, ReferenceResource.Country),
                Cities = GetRefResourceByName(refResources, ReferenceResource.City),
                Offices = GetRefResourceByName(refResources, ReferenceResource.Office),
                TaxDeclarations = GetRefResourceByName(refResources, ReferenceResource.TaxDeclaration),
                WorkPermitType1s = GetRefResourceByName(refResources, ReferenceResource.WorkPermitType1),
                WorkPermitType2s = GetRefResourceByName(refResources, ReferenceResource.WorkPermitType2)
            };

            return PartialView("_CreateEmployment", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> CreateEmployment(CreateEmploymentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await employeeManagementService.CreateEmploymentAsync(viewModel);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => x.Value.Errors.First().ErrorMessage);
            return Json(new { isSuccessful = false, errors });
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateEmployment(int employeeId)
        {
            var refNames = string.Format(
                "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
                ReferenceResource.LocalCompany,
                ReferenceResource.LocalJobTitle,
                ReferenceResource.EmploymentType,
                ReferenceResource.EmploymentStatus,
                ReferenceResource.JobCategory,
                ReferenceResource.Grade,
                ReferenceResource.GradeLevel,
                ReferenceResource.Country,
                ReferenceResource.City,
                ReferenceResource.Office,
                ReferenceResource.TaxDeclaration,
                ReferenceResource.WorkPermitType1,
                ReferenceResource.WorkPermitType2);
            var bviCompanies = await baseService.GetDataAsync<Company>(new GetAllCompanyCriteria { IsActive = true });
            var refResources = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNamesCriteria { RefNames = refNames });
            var viewModel = new CreateEmploymentViewModel
            {
                EmployeeId = employeeId,
                Companies = bviCompanies,
                LocalCompanies = GetRefResourceByName(refResources, ReferenceResource.LocalCompany),
                LocalJobTitles = GetRefResourceByName(refResources, ReferenceResource.LocalJobTitle),
                EmploymentTypes = GetRefResourceByName(refResources, ReferenceResource.EmploymentType),
                EmploymentStatuses = GetRefResourceByName(refResources, ReferenceResource.EmploymentStatus),
                JobCategories = GetRefResourceByName(refResources, ReferenceResource.JobCategory),
                Grades = GetRefResourceByName(refResources, ReferenceResource.Grade),
                GradeLevels = GetRefResourceByName(refResources, ReferenceResource.GradeLevel),
                Countries = GetRefResourceByName(refResources, ReferenceResource.Country),
                Cities = GetRefResourceByName(refResources, ReferenceResource.City),
                Offices = GetRefResourceByName(refResources, ReferenceResource.Office),
                TaxDeclarations = GetRefResourceByName(refResources, ReferenceResource.TaxDeclaration),
                WorkPermitType1s = GetRefResourceByName(refResources, ReferenceResource.WorkPermitType1),
                WorkPermitType2s = GetRefResourceByName(refResources, ReferenceResource.WorkPermitType2)
            };

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

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => x.Value.Errors.First().ErrorMessage);
            return Json(new { isSuccessful = false, errors });
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

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> GetEmployeeSuggestion(string idNo)
        {
            var criteria = new GetEmployeeSuggestionCriteria { IDNo = idNo };
            var employees = await baseService.GetDataAsync<EmployeeSuggestionViewModel>(criteria);

            return Json(employees);
        }

        //[HttpGet]
        //[HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Read)]
        //public async Task<IActionResult> GetEmployeeBankDetails(int employeeId)
        //{
        //}

        private static IEnumerable<ReferenceResourceModel> GetRefResourceByName(IEnumerable<ReferenceResourceModel> refResources, string refName)
        {
            return refResources.Where(x => x.RefName == refName);
        }
    }
}