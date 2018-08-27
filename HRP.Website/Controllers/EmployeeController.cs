namespace HRP.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Filters;
    using Core.Model;
    using Core.Services;
    using Criteria.Employee;
    using HRP.Core.Criteria;
    using HRP.Criteria;
    using HRP.Models.Currency;
    using HRP.Models.ServiceModels;
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
        private readonly IRijndaelService rijndaelService;

        public EmployeeController(
            IBaseService baseService,
            ISessionService sessionService,
            ICommonService commonService,
            IEmployeeService employeeService,
            IEmployeePayrollService payrollService,
            IRijndaelService rijndaelService) : base(baseService, sessionService)
        {
            this.commonService = commonService;
            this.employeeService = employeeService;
            this.payrollService = payrollService;
            this.rijndaelService = rijndaelService;
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
            string[] refNames = {
                ReferenceResource.LocalCompany,
                ReferenceResource.LocalJobTitle,
                ReferenceResource.EmploymentType,
                ReferenceResource.EmploymentStatus,
                ReferenceResource.Country,
                ReferenceResource.City,
                ReferenceResource.Office,
                ReferenceResource.WorkPermitType1,
                ReferenceResource.WorkPermitType2
            };

            IEnumerable<Company> companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.EmployeeProfile).ConfigureAwait(false);
            IEnumerable<JobCategory> jobCategories = await commonService.GetAllJobCategoryAsync().ConfigureAwait(false);
            IEnumerable<ReferenceResourceModel> refResources = await commonService.GetReferenceResourcesByNamesAsync(refNames).ConfigureAwait(false);
            IEnumerable<ReferenceResourceModel> localCompanies = commonService.FilterReferenceResources(refResources, ReferenceResource.LocalCompany);
            IEnumerable<ReferenceResourceModel> localJobTitles = commonService.FilterReferenceResources(refResources, ReferenceResource.LocalJobTitle);
            IEnumerable<ReferenceResourceModel> employmentTypes = commonService.FilterReferenceResources(refResources, ReferenceResource.EmploymentType);
            IEnumerable<ReferenceResourceModel> employmentStatuses = commonService.FilterReferenceResources(refResources, ReferenceResource.EmploymentStatus);
            IEnumerable<ReferenceResourceModel> countries = commonService.FilterReferenceResources(refResources, ReferenceResource.Country);
            IEnumerable<ReferenceResourceModel> cities = commonService.FilterReferenceResources(refResources, ReferenceResource.City);
            IEnumerable<ReferenceResourceModel> offices = commonService.FilterReferenceResources(refResources, ReferenceResource.Office);
            IEnumerable<ReferenceResourceModel> workPermitType1s = commonService.FilterReferenceResources(refResources, ReferenceResource.WorkPermitType1);
            IEnumerable<ReferenceResourceModel> workPermitType2s = commonService.FilterReferenceResources(refResources, ReferenceResource.WorkPermitType2);

            var viewModel = new CreateEmploymentViewModel
            {
                Companies = commonService.GetSelectList(companies, nameof(Company.CompanyId), nameof(Company.Code)),
                JobCategories = commonService.GetSelectList(jobCategories, nameof(JobCategory.JobCategoryId), nameof(JobCategory.Name)),
                LocalCompanies = commonService.GetSelectList(localCompanies, nameof(ReferenceResourceModel.ResourceId), nameof(ReferenceResourceModel.Value)),
                LocalJobTitles = commonService.GetSelectList(localJobTitles, nameof(ReferenceResourceModel.ResourceId), nameof(ReferenceResourceModel.Value)),
                EmploymentTypes = commonService.GetSelectList(employmentTypes, nameof(ReferenceResourceModel.ResourceId), nameof(ReferenceResourceModel.Value)),
                EmploymentStatuses = commonService.GetSelectList(employmentStatuses, nameof(ReferenceResourceModel.ResourceId), nameof(ReferenceResourceModel.Value)),
                Countries = commonService.GetSelectList(countries, nameof(ReferenceResourceModel.ResourceId), nameof(ReferenceResourceModel.Value)),
                Cities = commonService.GetSelectList(cities, nameof(ReferenceResourceModel.ResourceId), nameof(ReferenceResourceModel.Value)),
                Offices = commonService.GetSelectList(offices, nameof(ReferenceResourceModel.ResourceId), nameof(ReferenceResourceModel.Value)),
                WorkPermitType1s = commonService.GetSelectList(workPermitType1s, nameof(ReferenceResourceModel.ResourceId), nameof(ReferenceResourceModel.Value)),
                WorkPermitType2s = commonService.GetSelectList(workPermitType2s, nameof(ReferenceResourceModel.ResourceId), nameof(ReferenceResourceModel.Value))
            };
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
        [HRPPermission(FunctionKey = FunctionKey.EmployeePayroll, Policy = Policy.Read)]
        public async Task<IActionResult> Payroll(int id, CachingFilterViewModel filter)
        {
            var viewModel = await InitializePayrollViewModel(id);

            if (viewModel == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            viewModel.CachingFilter = filter;

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeePayroll, Policy = Policy.Host)]
        public async Task<IActionResult> EditPayroll(EmployeePayrollViewModel viewModel)
        {
            ModelState.Clear();
            DecryptData(viewModel);

            if (TryValidateModel(viewModel))
            {
                CommandResult result = await payrollService.UpdatePayroll(ConvertToServiceModel(viewModel), sessionService.UserId);
                return Json(new
                {
                    isSuccessful = result.IsSuccessful,
                    message = result.Message,
                    redirectTo = Url.Action("Index")
                });
            }
            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);
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
            var divisions = companyTrees.Where(t => t.CategoryId == (int)CompanyTreeType.Division);
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

        private async Task<EmployeePayrollViewModel> InitializePayrollViewModel(int employeeId)
        {
            var payroll = await payrollService.GetEmployeePayroll(employeeId, sessionService.UserId);

            if (payroll != null)
            {
                Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair> tuple = rijndaelService.GenerateRijndaelKeyValuePair();
                TempData[tuple.Item1.Key] = tuple.Item1.Value;
                TempData[tuple.Item2.Key] = tuple.Item2.Value;

                var viewModel = new EmployeePayrollViewModel
                {
                    EmployeeId = payroll.EmployeeId,
                    Code = payroll.Code,
                    CompanyId = payroll.CompanyId,
                    FullName = payroll.FullName,
                    Nickname = payroll.Nickname,
                    CanRead = payroll.CanRead,
                    CanUpdate = payroll.CanUpdate,
                    RijndaelKey = tuple.Item1,
                    RijndaelIV = tuple.Item2,
                    BasicSalary = InitializeBasicSalaryViewModel(payroll, tuple.Item1.Value, tuple.Item2.Value),
                    Allowance = InitializeAllowanceViewModel(payroll, tuple.Item1.Value, tuple.Item2.Value),
                    Deduction = InitializeDeductionViewModel(payroll, tuple.Item1.Value, tuple.Item2.Value)
                };

                var currencies = await baseService.GetDataAsync<CurrencyCodeModel>(new GetActiveCurrencyCodesCriteria());
                viewModel.CurrencyExchangeRates = await baseService.GetDataAsync<CurrencyExchangeRateModel>(new GetLatestExchangeRatesCriteria());
                viewModel.ChangeReasons = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNamesCriteria
                {
                    RefNames = ReferenceResource.BasicSalaryChangeReason,
                    IsActive = true
                });
                viewModel.CashAccounts = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNamesCriteria
                {
                    RefNames = ReferenceResource.PaymentTransactionCash,
                    IsActive = true
                });
                viewModel.BankTypes = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNamesCriteria
                {
                    RefNames = ReferenceResource.BankType,
                    IsActive = true
                });

                viewModel.HaveHostPermissionOnBankAccounts = sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.EmployeeBankDetails, Policy.Host);
                viewModel.Currencies = currencies;
                viewModel.VNDCurrencyId = currencies.FirstOrDefault(x => string.Equals(x.CurrencyCode, Constants.VNDCurrencyCode, StringComparison.OrdinalIgnoreCase))?.CurrencyId ?? 0;

                return viewModel;
            }

            return null;
        }

        private EmployeePayrollDeductionViewModel InitializeDeductionViewModel(
            EmployeePayrollModel payroll,
            string rijndaelKey,
            string rijndaelIV)
        {
            var viewModel = new EmployeePayrollDeductionViewModel
            {
                EmployeeId = payroll.Deduction.EmployeeId,
                RoomingCharges = new CurrencyAmountModel
                {
                    CurrencyId = payroll.Deduction.RoomingChargesCurrencyId,
                    PlainValue = payroll.Deduction.RoomingChargesAmount
                },
                FitnessFirst = new CurrencyAmountModel
                {
                    CurrencyId = payroll.Deduction.FitnessFirstCurrencyId,
                    PlainValue = payroll.Deduction.FitnessFirstAmount
                }
            };

            rijndaelService.EncryptObject(viewModel, rijndaelKey, rijndaelIV);
            rijndaelService.CleanPlainValue(viewModel);

            return viewModel;
        }

        private EmployeePayrollAllowanceViewModel InitializeAllowanceViewModel(
            EmployeePayrollModel payroll,
            string rijndaelKey,
            string rijndaelIV)
        {
            var viewModel = new EmployeePayrollAllowanceViewModel
            {
                EmployeeId = payroll.Allowance.EmployeeId,
                CurrencyId = payroll.Allowance.CurrencyId,
                AutoCalculateOn = payroll.Allowance.AutoCalculateOn,
                DutyMealPerDay = new SecurityData
                {
                    PlainValue = string.IsNullOrEmpty(payroll.Allowance.DutyMealAmount) ?
                        string.Empty :
                        (decimal.Parse(payroll.Allowance.DutyMealAmount) / payroll.BasicSalary.WorkingDays).ToString(CultureInfo.InvariantCulture),
                },
                DutyMeal = new CurrencyAmountModel
                {
                    CurrencyId = payroll.Allowance.DutyMealCurrencyId,
                    PlainValue = payroll.Allowance.DutyMealAmount
                },
                HomePassage = new CurrencyAmountModel
                {
                    CurrencyId = payroll.Allowance.HomePassageCurrencyId,
                    PlainValue = payroll.Allowance.HomePassageAmount
                },
                Housing = new CurrencyAmountModel
                {
                    CurrencyId = payroll.Allowance.HousingCurrencyId,
                    PlainValue = payroll.Allowance.HousingAmount
                },
                HPI = new CurrencyAmountModel
                {
                    CurrencyId = payroll.Allowance.HPICurrencyId,
                    PlainValue = payroll.Allowance.HPIAmount
                },
                MobilePhone = new CurrencyAmountModel
                {
                    CurrencyId = payroll.Allowance.MobilePhoneCurrencyId,
                    PlainValue = payroll.Allowance.MobilePhoneAmount
                },
                Secondment = new CurrencyAmountModel
                {
                    CurrencyId = payroll.Allowance.SecondmentCurrencyId,
                    PlainValue = payroll.Allowance.SecondmentAmount
                },
                Subsistence = new CurrencyAmountModel
                {
                    CurrencyId = payroll.Allowance.SubsistenceCurrencyId,
                    PlainValue = payroll.Allowance.SubsistenceAmount
                },
                Others = new CurrencyAmountModel
                {
                    CurrencyId = payroll.Allowance.OthersCurrencyId,
                    PlainValue = payroll.Allowance.OthersAmount
                },
                Cash = new PaymentTransactionModel
                {
                    AccountId = payroll.Allowance.CashAccountId,
                    CurrencyId = payroll.Allowance.CashCurrencyId,
                    PlainValue = payroll.Allowance.CashAmount
                },
                DirectBanking1 = new PaymentTransactionModel
                {
                    AccountId = payroll.Allowance.DirectBanking1AccountId,
                    AccountName = payroll.Allowance.DirectBanking1AccountName,
                    CurrencyId = payroll.Allowance.DirectBanking1CurrencyId,
                    PlainValue = payroll.Allowance.DirectBanking1Amount
                },
                DirectBanking2 = new PaymentTransactionModel
                {
                    AccountId = payroll.Allowance.DirectBanking2AccountId,
                    AccountName = payroll.Allowance.DirectBanking2AccountName,
                    CurrencyId = payroll.Allowance.DirectBanking2CurrencyId,
                    PlainValue = payroll.Allowance.DirectBanking2Amount
                }
            };

            rijndaelService.EncryptObject(viewModel, rijndaelKey, rijndaelIV);
            rijndaelService.CleanPlainValue(viewModel);

            return viewModel;
        }

        private EmployeePayrollBasicSalaryViewModel InitializeBasicSalaryViewModel(
            EmployeePayrollModel payroll,
            string rijndaelKey,
            string rijndaelIV)
        {
            var viewModel = new EmployeePayrollBasicSalaryViewModel
            {
                EmployeeId = payroll.BasicSalary.EmployeeId,
                EffectiveDate = payroll.BasicSalary.EffectiveDate,
                InitFirstTime = payroll.BasicSalary.InitFirstTime,
                ReasonId = payroll.BasicSalary.ReasonId,
                WorkingDays = payroll.BasicSalary.WorkingDays,
                AutoCalculateOn = payroll.BasicSalary.AutoCalculateOn,
                Salary = new CurrencyAmountModel
                {
                    CurrencyId = payroll.BasicSalary.CurrencyId,
                    PlainValue = payroll.BasicSalary.Salary
                },
                OldSalary = new CurrencyAmountModel
                {
                    CurrencyId = payroll.BasicSalary.CurrencyId,
                    PlainValue = payroll.BasicSalary.Salary
                },
                Cash = new PaymentTransactionModel
                {
                    AccountId = payroll.BasicSalary.CashAccountId,
                    CurrencyId = payroll.BasicSalary.CashCurrencyId,
                    PlainValue = payroll.BasicSalary.CashAmount
                },
                DirectBanking1 = new PaymentTransactionModel
                {
                    AccountId = payroll.BasicSalary.DirectBanking1AccountId,
                    AccountName = payroll.BasicSalary.DirectBanking1AccountName,
                    CurrencyId = payroll.BasicSalary.DirectBanking1CurrencyId,
                    PlainValue = payroll.BasicSalary.DirectBanking1Amount
                },
                DirectBanking2 = new PaymentTransactionModel
                {
                    AccountId = payroll.BasicSalary.DirectBanking2AccountId,
                    AccountName = payroll.BasicSalary.DirectBanking2AccountName,
                    CurrencyId = payroll.BasicSalary.DirectBanking2CurrencyId,
                    PlainValue = payroll.BasicSalary.DirectBanking2Amount
                },
                Ebanking = new PaymentTransactionModel
                {
                    AccountId = payroll.BasicSalary.EbankingAccountId,
                    AccountName = payroll.BasicSalary.EbankingAccountName,
                    CurrencyId = payroll.BasicSalary.EbankingCurrencyId,
                    PlainValue = payroll.BasicSalary.EbankingAmount
                }
            };

            rijndaelService.EncryptObject(viewModel, rijndaelKey, rijndaelIV);
            rijndaelService.CleanPlainValue(viewModel);

            return viewModel;
        }

        private void DecryptData(EmployeePayrollViewModel viewModel)
        {
            var rijndaelKey = (string)TempData.Peek(viewModel.RijndaelKey.Key);
            var rijndaelIV = (string)TempData.Peek(viewModel.RijndaelIV.Key);

            rijndaelService.DecryptObject(viewModel.BasicSalary, rijndaelKey, rijndaelIV);
            rijndaelService.DecryptObject(viewModel.Allowance, rijndaelKey, rijndaelIV);
            rijndaelService.DecryptObject(viewModel.Deduction, rijndaelKey, rijndaelIV);
        }

        private EmployeePayrollModel ConvertToServiceModel(EmployeePayrollViewModel model)
        {
            var payroll = new EmployeePayrollModel
            {
                EmployeeId = model.EmployeeId,
                BasicSalary = new EmployeePayrollBasicSalaryModel
                {
                    EmployeeId = model.BasicSalary.EmployeeId,
                    Salary = model.BasicSalary.Salary.PlainValue,
                    CurrencyId = model.BasicSalary.Salary.CurrencyId,
                    OldSalary = model.BasicSalary.OldSalary.PlainValue,
                    OldCurrencyId = model.BasicSalary.OldSalary.CurrencyId,
                    WorkingDays = model.BasicSalary.WorkingDays,
                    ReasonId = model.BasicSalary.ReasonId,
                    EffectiveDate = model.BasicSalary.EffectiveDate,
                    AutoCalculateOn = model.BasicSalary.AutoCalculateOn,
                    EbankingCurrencyId = model.BasicSalary.Ebanking.CurrencyId,
                    EbankingAmount = model.BasicSalary.Ebanking.PlainValue,
                    EbankingAccountId = model.BasicSalary.Ebanking.AccountId,
                    DirectBanking1CurrencyId = model.BasicSalary.DirectBanking1.CurrencyId,
                    DirectBanking1Amount = model.BasicSalary.DirectBanking1.PlainValue,
                    DirectBanking1AccountId = model.BasicSalary.DirectBanking1.AccountId,
                    DirectBanking2CurrencyId = model.BasicSalary.DirectBanking2.CurrencyId,
                    DirectBanking2Amount = model.BasicSalary.DirectBanking2.PlainValue,
                    DirectBanking2AccountId = model.BasicSalary.DirectBanking2.AccountId,
                    CashCurrencyId = model.BasicSalary.Cash.CurrencyId,
                    CashAmount = model.BasicSalary.Cash.PlainValue,
                    CashAccountId = model.BasicSalary.Cash.AccountId
                },
                Allowance = new EmployeePayrollAllowanceModel
                {
                    EmployeeId = model.Allowance.EmployeeId,
                    CurrencyId = model.Allowance.CurrencyId,
                    AutoCalculateOn = model.Allowance.AutoCalculateOn,
                    DirectBanking1CurrencyId = model.Allowance.DirectBanking1.CurrencyId,
                    DirectBanking1Amount = model.Allowance.DirectBanking1.PlainValue,
                    DirectBanking1AccountId = model.Allowance.DirectBanking1.AccountId,
                    DirectBanking2CurrencyId = model.Allowance.DirectBanking2.CurrencyId,
                    DirectBanking2Amount = model.Allowance.DirectBanking2.PlainValue,
                    DirectBanking2AccountId = model.Allowance.DirectBanking2.AccountId,
                    CashCurrencyId = model.Allowance.Cash.CurrencyId,
                    CashAmount = model.Allowance.Cash.PlainValue,
                    CashAccountId = model.Allowance.Cash.AccountId,
                    HomePassageCurrencyId = model.Allowance.HomePassage.CurrencyId,
                    HomePassageAmount = model.Allowance.HomePassage.PlainValue,
                    HousingCurrencyId = model.Allowance.Housing.CurrencyId,
                    HousingAmount = model.Allowance.Housing.PlainValue,
                    MobilePhoneCurrencyId = model.Allowance.MobilePhone.CurrencyId,
                    MobilePhoneAmount = model.Allowance.MobilePhone.PlainValue,
                    HPICurrencyId = model.Allowance.HPI.CurrencyId,
                    HPIAmount = model.Allowance.HPI.PlainValue,
                    DutyMealCurrencyId = model.Allowance.DutyMeal.CurrencyId,
                    DutyMealAmount = model.Allowance.DutyMeal.PlainValue,
                    SubsistenceCurrencyId = model.Allowance.Subsistence.CurrencyId,
                    SubsistenceAmount = model.Allowance.Subsistence.PlainValue,
                    SecondmentCurrencyId = model.Allowance.Secondment.CurrencyId,
                    SecondmentAmount = model.Allowance.Secondment.PlainValue,
                    OthersCurrencyId = model.Allowance.Others.CurrencyId,
                    OthersAmount = model.Allowance.Others.PlainValue
                },
                Deduction = new EmployeePayrollDeductionModel
                {
                    RoomingChargesCurrencyId = model.Deduction.RoomingCharges.CurrencyId,
                    RoomingChargesAmount = model.Deduction.RoomingCharges.PlainValue,
                    FitnessFirstCurrencyId = model.Deduction.FitnessFirst.CurrencyId,
                    FitnessFirstAmount = model.Deduction.FitnessFirst.PlainValue
                }
            };

            return payroll;
        }
    }
}