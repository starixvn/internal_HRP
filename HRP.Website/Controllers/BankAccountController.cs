namespace HRP.Website.Controllers
{
    using System.Linq;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Extensions;
    using Core.Filters;
    using Core.Model;
    using Core.Services;
    using Criteria.Bank;
    using Microsoft.AspNetCore.Mvc;
    using Models.Bank;
    using Services.Interfaces;

    [HRPAuthorize]
    public class BankAccountController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly IBankAccountService bankAccountService;
        private readonly IRefResourceService refResourceService;
        private readonly ISessionService sessionService;

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

            var bankTypes = refResourceService.FilterReferenceResources(refResources, ReferenceResource.BankType);
            var bankAccountTypes = refResourceService.FilterReferenceResources(refResources, ReferenceResource.BankAccountType);

            var data = bankAccountService.GetBankAccountsByEmployeeId(viewModel.EmployeeId);

            viewModel.BankAccounts = data.BankAccounts;
            viewModel.EmployeeName = data.EmployeeName;

            viewModel.Initialize(bankTypes, bankAccountTypes);

            viewModel.CompanyId = data.CompanyId;

            viewModel.CanCreate = sessionService.HasPermissionOnFunction(FunctionKey.EmployeeBankDetails, Policy.Host)
                                  && sessionService.HasPermissionOnCompany(
                                      viewModel.CompanyId,
                                      FunctionKey.EmployeeBankDetails,
                                      Policy.Host);

            // TODO: maybe cancreate and canedit is different
            viewModel.CanEdit = viewModel.CanCreate;

            if ((viewModel.SelectedBankTypes != null && !viewModel.SelectedBankTypes.Any())
                || viewModel.SelectedBankTypes == null)
            {
                viewModel.SelectedBankTypes = bankTypes.Select(x => x.ResourceId).ToArray();
            }

            viewModel.BankAccounts = bankAccountService.FilterBankAccounts(
                    viewModel.BankAccounts,
                    viewModel.SelectedBankTypes,
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

        [AcceptVerbs("Get", "Post")]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult CheckBankAccountExisted(BankAccount bankAccount)
        {
            var isExisted = bankAccountService.CheckIsBankAccountExisted(bankAccount);

            return isExisted ? Json(data: nameof(BankAccountError.AccountExisted)) : Json(data: true);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult CreateBankAccount(CreateBankAccountViewModel viewModel)
        {
            CommandResult result = null;

            if (ModelState.IsValid)
            {
                viewModel.EmployeeId = viewModel.BankAccount.EmployeeId;
                result = bankAccountService.CreateBankAccount(viewModel.BankAccount);

                if (result.IsSuccessful)
                {
                    var redirectViewModel =
                        new EmployeeBankAccountsViewModel { EmployeeId = viewModel.EmployeeId };
                    return RedirectToAction("EmployeeBankAccounts", redirectViewModel);
                }
            }

            PrepareCreateBankAccountViewModel(viewModel);

            if (result != null)
            {
                viewModel.Error = (BankAccountError)result.Code;
                ModelState.AddModelError("BankAccount.AccountName", viewModel.Error.GetDisplayName());
            }

            return View(viewModel);
        }

        private void PrepareCreateBankAccountViewModel(CreateBankAccountViewModel viewModel)
        {
            var data = bankAccountService.GetBankAccountsByEmployeeId(viewModel.EmployeeId);
            ////viewModel.EmployeeId = data.EmployeeId;
            viewModel.EmployeeName = data.EmployeeName;

            var refResources = refResourceService.GetReferenceResourcesByNames(refNames);

            var bankTypes = refResourceService.FilterReferenceResources(refResources, ReferenceResource.BankType);
            var bankAccountTypes = refResourceService.FilterReferenceResources(refResources, ReferenceResource.BankAccountType);

            viewModel.Initialize(bankTypes, bankAccountTypes);
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult EditBankAccount(int bankAccountId)
        {
            var bankAccount = bankAccountService.GetBankAccountById(bankAccountId);

            var data = bankAccountService.GetBankAccountsByEmployeeId(bankAccount.EmployeeId);

            var refResources = refResourceService.GetReferenceResourcesByNames(refNames);

            var bankTypes = refResourceService.FilterReferenceResources(refResources, ReferenceResource.BankType);
            var bankAccountTypes = refResourceService.FilterReferenceResources(refResources, ReferenceResource.BankAccountType);

            var viewModel = new EditBankAccountViewModel
            {
                EmployeeId = bankAccount.EmployeeId,
                EmployeeName = data.EmployeeName
            };

            viewModel.Initialize(bankAccount, bankTypes, bankAccountTypes);

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult EditBankAccount(EditBankAccountViewModel viewModel)
        {
            var result = bankAccountService.UpdateBankAccount(viewModel.BankAccount);

            if (result.IsSuccessful)
            {
                var redirectViewModel = new EmployeeBankAccountsViewModel { EmployeeId = viewModel.BankAccount.EmployeeId };
                return RedirectToAction("EmployeeBankAccounts", redirectViewModel);
            }

            return View(viewModel);
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
                return Json(result.Select(x => new { id = x.Id, text = x.AccountName }));
            }
        }
    }
}