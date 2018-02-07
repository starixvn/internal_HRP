namespace HRP.Website.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Extensions;
    using Core.Filters;
    using Core.Services;
    using Criteria.Bank;
    using Microsoft.AspNetCore.Mvc;
    using Models.Bank;
    using Services.Interfaces;
    using BankType = Models.Bank.BankType;

    [HRPAuthorize]
    public class BankAccountController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly IBankAccountService bankAccountService;
        private readonly IRefResourceService refResourceService;
        private readonly ISessionService sessionService;
        private const int MAX_BANK_ACCOUNT_PER_TYPE = 3;

        private readonly string[] refNames =
        {
            ReferenceResource.BankType,
            ReferenceResource.BankAccountType,
            ReferenceResource.ContraCoAccount
        };

        public BankAccountController(
            IBaseService baseService,
            IBankAccountService bankAccountService,
            IRefResourceService refResourceService,
            ISessionService sessionService) : base(baseService, sessionService)
        {
            this.baseService = baseService;
            this.bankAccountService = bankAccountService;
            this.refResourceService = refResourceService;
            this.sessionService = sessionService;
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Read)]
        [IgnoreAntiforgeryToken]
        ////[ResponseCache(CacheProfileName = nameof(CachingProfile.Default))]
        public IActionResult EmployeeBankAccounts(EmployeeBankAccountsViewModel viewModel)
        {
            var refResources = refResourceService.GetReferenceResourcesByNames(refNames);

            var bankTypes = refResourceService
                .FilterReferenceResources(refResources, ReferenceResource.BankType)
                .Select(x => new BankType(x));

            var bankAccountTypes = refResourceService
                .FilterReferenceResources(refResources, ReferenceResource.BankAccountType)
                .Select(x => new BankAccountType(x));

            var data = bankAccountService.GetBankAccountsByEmployeeId(viewModel.EmployeeId);

            viewModel.BankAccounts = data.BankAccounts;
            viewModel.EmployeeName = data.EmployeeName;

            viewModel.Initialize(bankTypes, bankAccountTypes);

            viewModel.CompanyId = data.CompanyId;

            viewModel.HaveCreatePermission = HasPermissionOnFunctionAndCompany(
                viewModel.CompanyId,
                FunctionKey.EmployeeBankDetails,
                Policy.Host);

            viewModel.CanEdit = viewModel.HaveCreatePermission;
            viewModel.CanCreate = viewModel.HaveCreatePermission;

            // Check maximum account per type
            var disabledBankTypes = GetDisabledBankTypes(data, bankTypes);
            if (bankTypes.Count() <= disabledBankTypes.Count)
            {
                viewModel.CanCreate = false;
            }

            viewModel.BankAccounts = bankAccountService.FilterBankAccounts(
                    viewModel.BankAccounts,
                    viewModel.SelectedBankType,
                    viewModel.IsActiveBankAccount)
                .ToList();

            return View(viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult CreateBankAccount(int employeeId)
        {
            var viewModel = new CreateBankAccountViewModel { EmployeeId = employeeId };

            PrepareCreateBankAccountViewModel(viewModel);

            return View(viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult CreateBankAccountInPayroll(int employeeId, int bankType)
        {
            var viewModel = new CreateBankAccountViewModel { EmployeeId = employeeId };

            PrepareCreateBankAccountViewModel(viewModel, bankType);

            viewModel.TypeOfForm = FormType.Modal;

            return PartialView("_CreateBankAccountInPayroll", viewModel);
        }

        [AcceptVerbs("Get", "Post")]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult CheckAccountNameExisted(BankAccount bankAccount)
        {
            var isExisted = bankAccountService.CheckIsAccountNameExisted(bankAccount);

            return isExisted ? Json(data: BankAccountError.AccountNameExisted.GetDisplayName()) : Json(data: true);
        }

        [AcceptVerbs("Get", "Post")]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult CheckAccountNumberExisted(BankAccount bankAccount)
        {
            var isExisted = bankAccountService.CheckIsAccountNumberExisted(bankAccount);

            return isExisted ? Json(data: BankAccountError.AccountNumberExisted.GetDisplayName()) : Json(data: true);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult CreateBankAccount(CreateBankAccountViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.EmployeeId = viewModel.BankAccount.EmployeeId;
                var result = bankAccountService.CreateBankAccount(viewModel.BankAccount);

                return Json(new
                {
                    isSuccessful = result.IsSuccessful,
                    message = ((BankAccountError)result.Code).GetDisplayName(),
                    redirectTo = Url.Action("EmployeeBankAccounts", "BankAccount", new { viewModel.EmployeeId })
                });
            }

            var errors = ModelState
                .Where(x => x.Value.Errors.Any())
                .Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });

            return Json(new { isSuccessful = false, errors });
        }

        private void PrepareCreateBankAccountViewModel(CreateBankAccountViewModel viewModel, int selectedBankType = 0)
        {
            var data = bankAccountService.GetBankAccountsByEmployeeId(viewModel.EmployeeId);

            viewModel.EmployeeName = data.EmployeeName;

            var refResources = refResourceService.GetReferenceResourcesByNames(refNames);

            var bankTypes = refResourceService
                .FilterReferenceResources(refResources, ReferenceResource.BankType)
                .Select(x => new BankType(x));

            var bankAccountTypes = refResourceService
                .FilterReferenceResources(refResources, ReferenceResource.BankAccountType)
                .Select(x => new BankAccountType(x));

            var disabledBankTypes = GetDisabledBankTypes(data, bankTypes);

            if (selectedBankType != 0)
            {
                // User create new bank account with pre-selected bank type in payroll page
                foreach (var bankType in bankTypes)
                {
                    if (bankType.Id != selectedBankType && disabledBankTypes.All(x => x.Id != bankType.Id))
                    {
                        disabledBankTypes.Add(bankType);
                    }
                }

                viewModel.HaveCreatePermission = HasPermissionOnFunctionAndCompany(
                    data.CompanyId,
                    FunctionKey.EmployeeBankDetails,
                    Policy.Host);

                viewModel.CanCreate = viewModel.HaveCreatePermission;

                if (bankTypes.Count() <= disabledBankTypes.Count)
                {
                    viewModel.CanCreate = false;
                }
            }

            viewModel.Initialize(bankTypes, bankAccountTypes, disabledBankTypes);
        }

        private static List<BankType> GetDisabledBankTypes(
            EmployeeBankAccounts data,
            IEnumerable<BankType> bankTypes,
            int currentBankType = 0,
            bool isCurrentBankAccountActive = true)
        {
            var disabledBankTypes = new List<BankType>();

            foreach (var bankType in bankTypes)
            {
                var countOfBankAccountInType = data.BankAccounts.Count(x => x.BankType == bankType.Id && x.IsActive);

                if (countOfBankAccountInType >= MAX_BANK_ACCOUNT_PER_TYPE && currentBankType != bankType.Id && isCurrentBankAccountActive)
                {
                    disabledBankTypes.Add(bankType);
                }
            }

            return disabledBankTypes;
        }

        private bool HasPermissionOnFunctionAndCompany(int companyId, string functionKey, Policy policy)
        {
            var hasPermissionOnFunction = sessionService.HasPermissionOnFunction(functionKey, policy);

            var hasPermissionOnCompany = sessionService.HasPermissionOnCompany(companyId, functionKey, policy);

            return hasPermissionOnFunction && hasPermissionOnCompany;
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult EditBankAccount(int bankAccountId)
        {
            var bankAccount = bankAccountService.GetBankAccountById(bankAccountId);

            var data = bankAccountService.GetBankAccountsByEmployeeId(bankAccount.EmployeeId);

            var refResources = refResourceService.GetReferenceResourcesByNames(refNames);

            var bankTypes = refResourceService
                .FilterReferenceResources(refResources, ReferenceResource.BankType)
                .Select(x => new BankType(x));

            var bankAccountTypes = refResourceService
                .FilterReferenceResources(refResources, ReferenceResource.BankAccountType)
                .Select(x => new BankAccountType(x));

            var disabledBankTypes = GetDisabledBankTypes(data, bankTypes, bankAccount.BankType, bankAccount.IsActive);

            var viewModel = new EditBankAccountViewModel
            {
                EmployeeId = bankAccount.EmployeeId,
                EmployeeName = data.EmployeeName
            };

            viewModel.Initialize(bankAccount, bankTypes, bankAccountTypes, disabledBankTypes);

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult EditBankAccount(EditBankAccountViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = bankAccountService.UpdateBankAccount(viewModel.BankAccount);

                return Json(new
                {
                    isSuccessful = result.IsSuccessful,
                    message = ((BankAccountError)result.Code).GetDisplayName(),
                    redirectTo = Url.Action("EmployeeBankAccounts", "BankAccount", new { viewModel.BankAccount.EmployeeId })
                });
            }

            var errors = ModelState
                .Where(x => x.Value.Errors.Any())
                .Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });

            return Json(new { isSuccessful = false, errors });
        }

        [HttpGet]
        public IActionResult GetEmployeeBankAccounts(int bankTypeId, int employeeId)
        {
            if (bankTypeId == (int)Core.Enumerations.BankType.ContraCoAccount)
            {
                var refResources = refResourceService.GetReferenceResourcesByNames(refNames);
                var result = refResourceService.FilterReferenceResources(refResources, ReferenceResource.ContraCoAccount);

                return Json(result.Select(x => new { id = x.ResourceId, text = x.Value }));
            }
            else
            {
                var result = baseService.GetData<BankAccount>(new SearchBankAccountCriteria { EmployeeId = employeeId, BankTypeId = bankTypeId });
                return Json(result.Where(x => x.IsActive).Select(x => new { id = x.Id, text = x.AccountName }));
            }
        }
    }
}