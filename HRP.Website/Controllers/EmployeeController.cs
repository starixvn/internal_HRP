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
    using Criteria.Employee;
    using HRP.Criteria;
    using HRP.Models.Currency;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Employee;
    using Models.Employee.Payroll;
    using Resources;
    using Services;

    public class EmployeeController : BaseController
    {
        private readonly ICommonService commonService;
        private readonly IEmployeeService employeeService;
        private readonly IEmployeePayrollService payrollService;

        public EmployeeController(
            IBaseService baseService,
            ISessionService sessionService,
            ICommonService commonService,
            IEmployeeService employeeService,
            IEmployeePayrollService payrollService) : base(baseService, sessionService)
        {
            this.commonService = commonService;
            this.employeeService = employeeService;
            this.payrollService = payrollService;
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeManagement, Policy = Policy.Read)]
        public async Task<IActionResult> Index(
            string departmentIds,
            string employmentStatusIds,
            int? employeeId,
            string displayName)
        {
            var viewModel = new EmployeeManagementViewModel
            {
                DepartmentIds = departmentIds,
                EmploymentStatusIds = employmentStatusIds,
                EmployeeId = employeeId,
                DisplayName = displayName,
                IsHostCreateEmployee = sessionService.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host)
            };

            viewModel.Employees = await employeeService.GetEmployeesAsync(viewModel);
            return View(viewModel);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeManagement, Policy = Policy.Read)]
        public async Task<IActionResult> GetDepartment(string departmentIds = null)
        {
            var criteria = new GetDepartmentByUserCriteria
            {
                UserId = sessionService.UserId,
            };

            var companyTrees = await baseService.GetDataAsync<CompanyTree>(criteria).ConfigureAwait(false);
            var hierarchyCompanies = GetHierarchyCompanies(companyTrees, departmentIds);

            return Json(hierarchyCompanies);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeManagement, Policy = Policy.Read)]
        public async Task<IActionResult> GetEmployeeStatus(string employmentStatusIds = null)
        {
            var criteria = new GetEmployeeStatusByUserCriteria
            {
                UserId = sessionService.UserId
            };
            var employeeStatus = await baseService.GetDataAsync<MultiSelectViewModel>(criteria).ConfigureAwait(false);

            return Json(CombineEmployeeStatus(employeeStatus.ToList(), employmentStatusIds));
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
        public async Task<IActionResult> CreateEmployee(CachingFilterViewModel filter)
        {
            var tuple = await employeeService.GetDataToCreateEmployeeAsync();
            var genderSelectList = tuple.Item1;
            var nationalSelectList = tuple.Item2;
            var maritalStatusSelectList = tuple.Item3;
            var noOfChildrenSelectList = tuple.Item4;

            var viewModel = new CreateEmployeeViewModel
            {
                GenderList = genderSelectList,
                NationalityList = nationalSelectList,
                MaritalStatusList = maritalStatusSelectList,
                NoOfChildrenList = noOfChildrenSelectList,
                CachingFilter = filter
            };

            return View("CreateEmployee", viewModel);
        }

        [Route("Employee/Create/PersonalDetails")]
        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public IActionResult CreatePersonalDetails(CreateEmployeeViewModel employee)
        {
            CommandResult validatePicture = employeeService.CheckValidPicture(employee.ProfilePicture);
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

        [Route("Employee/Create/Employment")]
        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> CreateEmployment(CachingFilterViewModel filter)
        {
            CreateEmploymentViewModel viewModel = await employeeService.GetDataToCreateEmploymentAsync();
            viewModel.CachingFilter = filter;

            return PartialView("_CreateEmployment", viewModel);
        }

        [Route("Employee/Create/Employment")]
        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> CreateEmployment(CreateEmploymentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await employeeService.CreateEmployeeAsync(viewModel);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);
            return Json(new { isSuccessful = false, errors });
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Read)]
        public async Task<IActionResult> UpdateEmployee(int id, CachingFilterViewModel filter)
        {
            var employee = await employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            var tuple = await employeeService.GetDataToCreateEmployeeAsync();
            var genderSelectList = tuple.Item1;
            var nationalSelectList = tuple.Item2;
            var maritalStatusSelectList = tuple.Item3;
            var noOfChildrenSelectList = tuple.Item4;
            var emergencyContact = await employeeService.GetEmergencyContactByEmployeeIdAsync(id);

            var viewModel = new CreateEmployeeViewModel
            {
                EmployeeId = employee.EmployeeId,
                Nickname = employee.Nickname,
                FullName = employee.FullName,
                Gender = employee.Gender,
                DateOfBirth = employee.DateOfBirth,
                PlaceOfBirth = employee.PlaceOfBirth,
                Nationality = employee.Nationality,
                NRICNo = employee.NRICNo,
                MaritalStatus = employee.MaritalStatus,
                NoOfChildren = employee.NoOfChildren,
                OverseasPhoneNo = employee.OverseasPhoneNo,
                LocalPhoneNo = employee.LocalPhoneNo,
                OtherPhoneNo = employee.OtherPhoneNo,
                PassportNo = employee.PassportNo,
                DateIssue = employee.DateIssue,
                ExpiredDate = employee.ExpiredDate,
                PlaceOfIssue = employee.PlaceOfIssue,
                Skype = employee.Skype,
                PersonalEmail = employee.PersonalEmail,
                PermanentAddress = employee.PermanentAddress,
                ResidenceAddress = employee.ResidenceAddress,
                MailingAddress = employee.MailingAddress,
                ProfilePic = employee.ProfilePic,
                IsHost = employee.IsHost,
                EmergencyContact = emergencyContact,
                GenderList = genderSelectList,
                NationalityList = nationalSelectList,
                MaritalStatusList = maritalStatusSelectList,
                NoOfChildrenList = noOfChildrenSelectList,
                CachingFilter = filter
            };

            return View("UpdateEmployee", viewModel);
        }

        [Route("Employee/Update/PersonalDetails")]
        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> UpdatePersonalDetails(CreateEmployeeViewModel employee)
        {
            var validatePicture = employeeService.CheckValidPicture(employee.ProfilePicture);
            if (!validatePicture.IsSuccessful)
            {
                ModelState.AddModelError(nameof(employee.ProfilePicture), validatePicture.Message);
            }

            if (ModelState.IsValid)
            {
                var commandResult = await employeeService.UpdatePersonalDetailsAsync(employee);
                var returnMessage = commandResult.IsSuccessful ? string.Format(Message.UpdateEmployeeSuccessful, employee.Nickname) : commandResult.Message;

                return Json(new { isSuccessful = commandResult.IsSuccessful, message = returnMessage });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        [Route("Employee/Update/Employment")]
        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Read)]
        public async Task<IActionResult> UpdateEmployment(int id, CachingFilterViewModel filter)
        {
            UpdateEmploymentViewModel viewModel = await employeeService.GetDataToUpdateEmploymentAsync(id);

            if (viewModel == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            viewModel.CachingFilter = filter;
            viewModel.IsHost = sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.EmployeeProfile, Policy.Host);

            return PartialView("_UpdateEmployment", viewModel);
        }

        [Route("Employee/Update/Employment")]
        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateEmployment(UpdateEmploymentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await employeeService.UpdateEmploymentAsync(viewModel);

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
            IEnumerable<Division> divisions = await commonService.GetDivisionByCompanyIdAsync(companyId);
            return Json(divisions);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> GetDepartmentByDivisionId(int divisionId)
        {
            IEnumerable<Department> departments = await commonService.GetDepartmentByDivisionIdAsync(divisionId);
            return Json(departments);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> GetCostCenterByDepartmentId(int departmentId)
        {
            IEnumerable<CostCenter> costCenters = await commonService.GetCostCenterByDepartmentIdAsync(departmentId);
            return Json(costCenters);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> GetSectionByCostCenterId(int costCenterId)
        {
            IEnumerable<Section> sections = await commonService.GetSectionByCostCenterIdAsync(costCenterId);
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
            IEnumerable<GradeLevel> gradeLevels = await commonService.GetGradeLevelByJobCategoryIdAsync(jobCategoryId);
            return Json(gradeLevels);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> GetJobGradeByGradeLevelId(int gradeLevelId)
        {
            IEnumerable<JobGrade> jobGrades = await commonService.GetJobGradeByGradeLevelIdAsync(gradeLevelId);
            return Json(jobGrades);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> GetCityByCountryId(int countryId)
        {
            IEnumerable<City> cities = await commonService.GetCityByCountryIdAsync(countryId);
            return Json(cities);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeProfile, Policy = Policy.Host)]
        public async Task<IActionResult> GetOfficeByCityId(int cityId)
        {
            IEnumerable<Office> offices = await commonService.GetOfficeByCityIdAsync(cityId);
            return Json(offices);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeePayroll, Policy = Policy.Read)]
        public async Task<IActionResult> Payroll(int id, CachingFilterViewModel filter)
        {
            EmployeePayrollViewModel viewModel = await payrollService.GetEmployeePayroll(id, sessionService.UserId);

            if (viewModel == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            var currencies = await baseService.GetDataAsync<CurrencyCodeModel>(new GetActiveCurrencyCodesCriteria());

            viewModel.CachingFilter = filter;
            viewModel.HaveHostPermissionOnBankAccounts = sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.EmployeeBankDetails, Policy.Host);
            viewModel.Currencies = currencies;
            viewModel.VNDCurrencyId = currencies.FirstOrDefault(x => string.Equals(x.CurrencyCode, Constants.VNDCurrencyCode, StringComparison.OrdinalIgnoreCase))?.CurrencyId ?? 0;
            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeePayroll, Policy = Policy.Host)]
        public async Task<IActionResult> EditPayroll(EmployeePayrollViewModel model)
        {
            if (ModelState.IsValid)
            {
                CommandResult result = await payrollService.UpdatePayroll(model, sessionService.UserId);
                return Json(new
                {
                    isSuccessful = result.IsSuccessful,
                    message = result.Message,
                    redirectTo = Url.Action("Index")
                });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [HttpGet]
        public async Task<IActionResult> CheckEmployeePayrollChangedInMonth(DateTime month, int employeeId)
        {
            var result = await baseService.GetDataAsync<int>(new CheckEmployeePayrollChangedInMonthCriteria { Month = month, EmployeeId = employeeId });
            return Json(result.Any());
        }

        private IEnumerable<MultiSelectViewModel> GetHierarchyCompanies(IEnumerable<CompanyTree> companyTrees, string departmentIds)
        {
            var divisions = companyTrees.Where(t => t.CategoryId == (int)CompanyTreeType.Divison);
            var departments = companyTrees.Where(t => t.CategoryId == (int)CompanyTreeType.Department);
            var selectedDepartments = (departmentIds ?? string.Empty).Split(',');

            var companyData = companyTrees.Where(t => t.CategoryId == (int)CompanyTreeType.Company).Select(t => new MultiSelectViewModel
            {
                Value = t.Id.ToString(),
                Text = t.Code,
                ParentId = t.ParentId.ToString(),
                Checked = false,
                Items = divisions.Where(di => di.ParentId == t.Id).Select(di => new MultiSelectViewModel
                {
                    Value = di.Id.ToString(),
                    Text = di.Code,
                    ParentId = di.ParentId.ToString(),
                    Checked = departmentIds == null || !departments.Where(y => y.ParentId == di.Id).Any(x => !selectedDepartments.Contains(x.Id.ToString())),
                    Items = departments.Where(de => de.ParentId == di.Id).Select(de => new MultiSelectViewModel
                    {
                        Value = de.Id.ToString(),
                        Text = de.Code,
                        ParentId = de.ParentId.ToString(),
                        Checked = departmentIds == null || selectedDepartments.Contains(de.Id.ToString())
                    }).ToList()
                }).ToList()
            }).ToList();

            foreach (var company in companyData)
            {
                company.Checked = !company.Items.Any(t => !t.Checked);
            }

            return companyData;
        }

        private static IList<MultiSelectViewModel> CombineEmployeeStatus(IList<MultiSelectViewModel> employeeStatus, string employmentStatusIds)
        {
            var checkedEmployeeIds = (employmentStatusIds ?? string.Empty).Split(',');
            foreach (var employee in employeeStatus)
            {
                employee.Checked = employmentStatusIds == null || checkedEmployeeIds.Contains(employee.Value);
            }

            return employeeStatus;
        }
    }
}