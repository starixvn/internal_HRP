namespace HRP.Website.Controllers
{
    using System;
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
    using HRP.Models.Currency;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Bank;
    using Models.EmployeeManagement;
    using Services;
    using Services.Interfaces;

    [HRPAuthorize]
    public class EmployeeManagementController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IBaseService baseService;
        private readonly IEmployeeManagementService employeeManagementService;
        private readonly IBankAccountService bankAccountService;

        public EmployeeManagementController(
            ISessionService sessionService,
            IBaseService baseService,
            IEmployeeManagementService employeeManagementService,
            IBankAccountService bankAccountService)
        {
            this.sessionService = sessionService;
            this.baseService = baseService;
            this.employeeManagementService = employeeManagementService;
            this.bankAccountService = bankAccountService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> CreateEmployee()
        {
            var viewModel = await employeeManagementService.InitDataForNewEmployeeAsync();

            return View("CreateEmployee", viewModel);
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
                var commandResult = await employeeManagementService.CreateEmployeeAsync(employee);

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = commandResult.IsSuccessful ? "create Success!" : commandResult.Message });
            }

            var errorList = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
            );

            return Json(new { isSuccessful = false, errors = errorList });
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public IActionResult EditEmployee(int id)
        {
            var viewModel = employeeManagementService.GetEmployeeById(id);

            if (viewModel == null)
            {
                return RedirectToAction("Index");
            }

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

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = commandResult.IsSuccessful ? "edit Success!" : commandResult.Message });
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
                "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}",
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
                ReferenceResource.WorkPermitType2,
                ReferenceResource.BankType,
                ReferenceResource.BankAccountType);
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

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);
            return Json(new { isSuccessful = false, errors });
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateEmployment(int employeeId)
        {
            var refNames = string.Format(
                "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}",
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
            viewModel.JobCategories = GetRefResourceByName(refResources, ReferenceResource.JobCategory);
            viewModel.Grades = GetRefResourceByName(refResources, ReferenceResource.Grade);
            viewModel.GradeLevels = GetRefResourceByName(refResources, ReferenceResource.GradeLevel);
            viewModel.Countries = GetRefResourceByName(refResources, ReferenceResource.Country);
            viewModel.Cities = GetRefResourceByName(refResources, ReferenceResource.City);
            viewModel.Offices = GetRefResourceByName(refResources, ReferenceResource.Office);
            viewModel.TaxDeclarations = GetRefResourceByName(refResources, ReferenceResource.TaxDeclaration);
            viewModel.WorkPermitType1s = GetRefResourceByName(refResources, ReferenceResource.WorkPermitType1);
            viewModel.WorkPermitType2s = GetRefResourceByName(refResources, ReferenceResource.WorkPermitType2);
            viewModel.CommonReasons = GetRefResourceByName(refResources, ReferenceResource.CommonReasons);
            viewModel.GroupDateReasons = GetRefResourceByName(refResources, ReferenceResource.GroupDateReasons);
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

        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Read)]
        public async Task<IActionResult> EditEmployeeBankDetails(EditEmployeeBankDetailsViewModel viewModel)
        {
            var employee = employeeManagementService.GetEmployeeById(viewModel.EmployeeId);

            var refNames = string.Format(
                "{0},{1}",
                ReferenceResource.BankType,
                ReferenceResource.BankAccountType);

            var refResources =
                await baseService.GetDataAsync<ReferenceResourceModel>(
                    new GetRefResourceByNamesCriteria { RefNames = refNames });

            var bankTypes = GetRefResourceByName(refResources, ReferenceResource.BankType);
            ////var bankAccountTypes = GetRefResourceByName(refResources, ReferenceResource.BankAccountType);

            viewModel.Initialize(employee.FullName, bankTypes);

            var bankAccounts = bankAccountService.GetBankAccountsByEmployeeId(viewModel.EmployeeId);
            viewModel.BankAccounts = bankAccountService
                .FilterBankAccounts(bankAccounts, viewModel.SelectedBankType, viewModel.IsActiveBankAccount)
                .ToList();

            return View(viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public async Task<IActionResult> CreateNewBankAccount(int employeeId)
        {
            var employee = employeeManagementService.GetEmployeeById(employeeId);

            var refNames = string.Format(
                "{0},{1}",
                ReferenceResource.BankType,
                ReferenceResource.BankAccountType);

            var refResources =
                await baseService.GetDataAsync<ReferenceResourceModel>(
                    new GetRefResourceByNamesCriteria { RefNames = refNames });

            var bankTypes = GetRefResourceByName(refResources, ReferenceResource.BankType);
            var bankAccountTypes = GetRefResourceByName(refResources, ReferenceResource.BankAccountType);

            var viewModel = new CreateNewBankAccountViewModel
            {
                EmployeeId = employeeId,
                EmployeeName = employee.FullName
            };

            viewModel.Initialize(employee.FullName, bankTypes, bankAccountTypes);

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult CreateNewBankAccount(CreateNewBankAccountViewModel viewModel)
        {
            var result = bankAccountService.CreateBankAccount(viewModel.BankAccount);

            if (result.IsSuccessful)
            {
                return RedirectToAction("CreateNewBankAccount", viewModel.EmployeeId);
            }

            return View(viewModel);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeePayroll, Policy = Policy.Read)]
        public IActionResult Payroll(int id)
        {
            var viewModel = new EmployeePayrollViewModel
            {
                BasicSalary = new EmployeePayrollBasicSalaryModel(),
                CurrencyExchangeRates = baseService.GetData<CurrencyExchangeRateModel>(new GetExchangeRateCriteria { Month = DateTime.Today.Month, Year = DateTime.Today.Year }),
                Currencies = baseService.GetData<CurrencyCodeModel>(new GetActiveCurrencyCodesCriteria()),
            };

            return View(viewModel);
        }

        private static IEnumerable<ReferenceResourceModel> GetRefResourceByName(IEnumerable<ReferenceResourceModel> refResources, string refName)
        {
            return refResources.Where(x => x.RefName == refName);
        }
    }
}