namespace HRP.Website.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Filters;
    using HRP.Core.Model;
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

                return RedirectToAction("Index");
            }

            return View("PersonalDetail", employee);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> CreateEmployment(int employeeId)
        {
            var refNames = string.Format(
                "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
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
            var bviCompanies = await baseService.GetDataAsync<Company>(new GetAllCompanyCriteria { Type = CompanyType.PVI, IsActive = true });
            var localCompanies = await baseService.GetDataAsync<Company>(new GetAllCompanyCriteria { Type = CompanyType.Local, IsActive = true });
            var refResources = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNamesCriteria { RefNames = refNames });
            var viewModel = new CreateEmploymentViewModel
            {
                EmployeeId = employeeId,
                BVICompanies = bviCompanies,
                LocalCompanies = localCompanies,
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
        public IActionResult CreateEmployment(CreateEmploymentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var criteria = new CreateEmploymentCriteria
                {
                    EmployeeId = viewModel.EmployeeId,
                    BVICompanyId = viewModel.BVICompanyId,
                    LocalCompanyId = viewModel.LocalCompanyId,
                    BVIJobTitle = viewModel.BVIJobTitle,
                    LocalJobTitleId = viewModel.LocalJobTitleId,
                    CompanyCode = viewModel.CompanyCode,
                    EmployeeCode = viewModel.EmployeeCode,
                    EmploymentType = viewModel.EmploymentType,
                    EmploymentStatus = viewModel.EmploymentStatus,
                    JobCategoryId = viewModel.JobCategoryId,
                    GradeId = viewModel.GradeId,
                    GradeLevelId = viewModel.GradeLevelId,
                    DivisionId = viewModel.DivisionId,
                    DepartmentId = viewModel.DepartmentId,
                    SectionId = viewModel.SectionId,
                    CostCenterId = viewModel.CostCenterId,
                    GroupStartDate = viewModel.GroupStartDate,
                    CompanyStartDate = viewModel.CompanyStartDate,
                    Email = viewModel.Email,
                    CountryId = viewModel.CountryId,
                    CityId = viewModel.CountryId,
                    OfficeId = viewModel.OfficeId,
                    DST = viewModel.DST,
                    DBT = viewModel.DBT,
                    TaxIDNo = viewModel.TaxIDNo,
                    SSSNo = viewModel.SSSNo,
                    HealthInsNo = viewModel.HealthInsNo,
                    HDMFNo = viewModel.HDMFNo,
                    WorkPermitCompanyId = viewModel.WorkPermit.LocalCompanyId,
                    Type1 = viewModel.WorkPermit.Type1,
                    StartDateType1 = viewModel.WorkPermit.StartDateType1,
                    EndDateType1 = viewModel.WorkPermit.EndDateType1,
                    Type2 = viewModel.WorkPermit.Type2,
                    StartDateType2 = viewModel.WorkPermit.StartDateType2,
                    EndDateType2 = viewModel.WorkPermit.EndDateType2,
                    FullNameBVISalary = viewModel.Paymaster.FullNameBVISalary,
                    NickNameBVISalary = viewModel.Paymaster.NickNameBVISalary,
                    FullNameBVIAllowance = viewModel.Paymaster.FullNameBVIAllowance,
                    NickNameBVIAllowance = viewModel.Paymaster.NickNameBVIAllowance,
                    FullNameLocalSalary = viewModel.Paymaster.FullNameLocalSalary,
                    NickNameLocalSalary = viewModel.Paymaster.NickNameLocalSalary,
                    FullNameAdministrator = viewModel.EPA.FullNameAdministrator,
                    NickNameAdministrator = viewModel.EPA.NickNameAdministrator,
                    FullNameEvaluator1 = viewModel.EPA.FullNameEvaluator1,
                    NickNameEvaluator1 = viewModel.EPA.NickNameEvaluator1,
                    FullNameEvaluator2 = viewModel.EPA.FullNameEvaluator2,
                    NickNameEvaluator2 = viewModel.EPA.NickNameEvaluator2,
                    FullNameReviewer = viewModel.EPA.FullNameReviewer,
                    NickNameReviewer = viewModel.EPA.NickNameReviewer,
                    FullNameManagement = viewModel.EPA.FullNameManagement,
                    NickNameManagement = viewModel.EPA.NickNameManagement,
                    FullNameIncrement = viewModel.EPA.FullNameIncrement,
                    NickNameIncrement = viewModel.EPA.NickNameIncrement,
                    FullNameBVIBonus = viewModel.EPA.FullNameBVIBonus,
                    NickNameBVIBonus = viewModel.EPA.NickNameBVIBonus,
                    FullNameLocalBonus = viewModel.EPA.FullNameLocalBonus,
                    NickNameLocalBonus = viewModel.EPA.NickNameLocalBonus,
                    FullNamePFileAdmin = viewModel.FullNamePFileAdmin,
                    NickNamePFileAdmin = viewModel.NickNamePFileAdmin,
                    PDFPassword = viewModel.PDFPassword,
                    CreatedBy = sessionService.UserId
                };

                CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

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

        private static IEnumerable<ReferenceResourceModel> GetRefResourceByName(IEnumerable<ReferenceResourceModel> refResources, string refName)
        {
            return refResources.Where(x => x.RefName == refName);
        }
    }
}