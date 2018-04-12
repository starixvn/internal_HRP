namespace HRP.Website.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Filters;
    using Core.Model;
    using Core.Services;
    using Criteria.BankAccount;
    using Microsoft.AspNetCore.Mvc;
    using Models.BankAccount;
    using Models;
    using Services;

    [HRPAuthorize]
    public class BankAccountController : BaseController
    {
        private readonly IBankAccountService bankAccountService;
        private readonly IEmployeeService employeeService;
        private readonly IRefResourceService refResourceService;

        private readonly string[] refNames =
        {
            ReferenceResource.BankType,
            ReferenceResource.TypeOfBankAccount
        };

        public BankAccountController(
            IBaseService baseService,
            IEmployeeService employeeService,
            IBankAccountService bankAccountService,
            IRefResourceService refResourceService,
            ISessionService sessionService) : base(baseService, sessionService)
        {
            this.employeeService = employeeService;
            this.bankAccountService = bankAccountService;
            this.refResourceService = refResourceService;
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Read)]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> EmployeeBankAccounts(EmployeeBankAccountsViewModel viewModel)
        {
            Employee employee = await employeeService.GetEmployeeBasicInfoByIdAsync(viewModel.EmployeeId);
            viewModel.CompanyId = employee.CompanyId;
            viewModel.EmployeeName = employee.NickName;

            viewModel.BankTypeResources = refResourceService.GetReferenceResourcesByName(ReferenceResource.BankType);
            viewModel.BankAccounts = await bankAccountService.GetBankAccountsByEmployeeIdAsync(viewModel.EmployeeId, viewModel.BankTypeId, viewModel.IsActive);
            viewModel.HaveHostPermission = sessionService.HasPermissionOnCompany(employee.CompanyId, FunctionKey.EmployeeBankDetails, Policy.Host);
            viewModel.BankAccountsLimitReached =
                await baseService.GetDataEntityAsync<bool>(new CheckBankAccountLimitReachedCriteria { EmployeeId = employee.EmployeeId });

            return View(viewModel);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public async Task<IActionResult> GetEmployeeBankAccounts(int bankTypeId, int employeeId)
        {
            if (bankTypeId == (int)BankTypes.ContraCoAccount)
            {
                var refResources = refResourceService.GetReferenceResourcesByName(ReferenceResource.ContraCoAccount);
                return Json(refResources.Select(x => new { id = x.ResourceId, text = x.Value }));
            }

            var criteria = new SearchBankAccountCriteria
            {
                EmployeeId = employeeId,
                BankTypeId = bankTypeId,
                IsActive = true
            };

            var result = await baseService.GetDataAsync<BankAccount>(criteria);
            return Json(result.Select(x => new { id = x.BankAccountId, text = x.AccountName }));
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public async Task<IActionResult> CreateBankAccount(int employeeId)
        {
            var viewModel = new CreateBankAccountViewModel { EmployeeId = employeeId };

            await PrepareCreateBankAccountViewModel(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public async Task<IActionResult> CreateBankAccount(CreateBankAccountViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await bankAccountService.CreateBankAccountAsync(viewModel);
                viewModel.IsSuccessful = commandResult.IsSuccessful;

                if (!viewModel.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, commandResult.Message);
                }
            }

            await PrepareCreateBankAccountViewModel(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public async Task<IActionResult> CreateBankAccountInPayroll(CreateBankAccountViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await bankAccountService.CreateBankAccountAsync(viewModel);
                viewModel.IsSuccessful = commandResult.IsSuccessful;

                if (!viewModel.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, commandResult.Message);
                }
            }

            await PrepareCreateBankAccountViewModel(viewModel);
            return PartialView("_CreateBankAccountInPayroll", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public async Task<IActionResult> CreateBankAccountInPayroll(int employeeId, int bankType)
        {
            var viewModel = new CreateBankAccountViewModel { EmployeeId = employeeId };

            await PrepareCreateBankAccountViewModel(viewModel, bankType);
            return PartialView("_CreateBankAccountInPayroll", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public async Task<IActionResult> EditBankAccount(int bankAccountId)
        {
            BankAccount bankAccount = await bankAccountService.GetBankAccountByIdAsync(bankAccountId);

            var viewModel = new EditBankAccountViewModel
            {
                EmployeeId = bankAccount.EmployeeId,
                NickName = bankAccount.NickName,
                BankAccountId = bankAccount.BankAccountId,
                AccountName = bankAccount.AccountName,
                AccountNumber = bankAccount.AccountNumber,
                AccountTypeId = bankAccount.AccountType,
                BankAddress = bankAccount.BankAddress,
                BankBranch = bankAccount.BankBranch,
                BankName = bankAccount.BankName,
                BankSwiftCode = bankAccount.BankSwiftCode,
                BankTypeId = bankAccount.BankType,
                BeneficiaryName = bankAccount.BeneficiaryName,
                IsActive = bankAccount.IsActive
            };

            var refResources = refResourceService.GetReferenceResourcesByNames(refNames);
            viewModel.BankTypeResources = refResourceService.FilterReferenceResources(refResources, ReferenceResource.BankType);
            viewModel.BankAccountTypeResources = refResourceService.FilterReferenceResources(refResources, ReferenceResource.TypeOfBankAccount);

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public async Task<IActionResult> EditBankAccount(EditBankAccountViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                CommandResult result = await bankAccountService.EditBankAccountAsync(viewModel);
                viewModel.IsSuccessful = result.IsSuccessful;

                if (!viewModel.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            var refResources = refResourceService.GetReferenceResourcesByNames(refNames);
            viewModel.BankTypeResources = refResourceService.FilterReferenceResources(refResources, ReferenceResource.BankType);
            viewModel.BankAccountTypeResources = refResourceService.FilterReferenceResources(refResources, ReferenceResource.TypeOfBankAccount);

            return View(viewModel);
        }

        private async Task PrepareCreateBankAccountViewModel(BaseFormBankAccountViewModel viewModel, int selectedBankType = 1)
        {
            Employee employee = await employeeService.GetEmployeeBasicInfoByIdAsync(viewModel.EmployeeId);

            viewModel.NickName = employee.NickName;
            viewModel.BankTypeId = selectedBankType;

            // TODO: disable over limit bank types (or change spec)

            var refResources = refResourceService.GetReferenceResourcesByNames(refNames);
            viewModel.BankTypeResources = refResourceService.FilterReferenceResources(refResources, ReferenceResource.BankType);
            viewModel.BankAccountTypeResources = refResourceService.FilterReferenceResources(refResources, ReferenceResource.TypeOfBankAccount);
            viewModel.HaveHostPermission = sessionService.HasPermissionOnCompany(employee.CompanyId, FunctionKey.EmployeeBankDetails, Policy.Host);
        }
    }
}