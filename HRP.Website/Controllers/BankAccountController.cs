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
    using Criteria.BankAccount;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.BankAccount;
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

            viewModel.BankTypeResources = await refResourceService.GetReferenceResourcesByNamesAsync(new[] { ReferenceResource.BankType });
            viewModel.BankAccounts = await bankAccountService.GetBankAccountsByEmployeeIdAsync(viewModel.EmployeeId, viewModel.BankTypeId, viewModel.IsActive);
            viewModel.HaveHostPermission = sessionService.HasPermissionOnCompany(employee.CompanyId, FunctionKey.EmployeeBankDetails, Policy.Host);

            ////viewModel.BankAccountsLimitReached =
            ////    await baseService.GetDataEntityAsync<bool>(new CheckBankAccountLimitReachedCriteria { EmployeeId = employee.EmployeeId });

            return View(viewModel);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public async Task<IActionResult> GetEmployeeBankAccounts(int bankTypeId, int employeeId)
        {
            var criteria = new SearchBankAccountCriteria
            {
                EmployeeId = employeeId,
                BankTypeId = bankTypeId,
                IsActive = true
            };

            var result = await baseService.GetDataAsync<BankAccount>(criteria);
            return Json(result.Select(x => new { id = x.BankAccountId, text = x.PaymentTransaction }));
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public async Task<IActionResult> GetPaymentTransactions(int bankTypeId, int employeeId, bool isActive, int bankAccountId = 0)
        {
            var refResourceName = bankTypeId == (int)BankTypes.Ebanking
                ? ReferenceResource.PaymentTransactionEbanking
                : ReferenceResource.PaymentTransactionDirectBanking;

            var results = await refResourceService.GetReferenceResourcesByNamesAsync(new[] { refResourceName });
            var bankAccounts = await bankAccountService.GetBankAccountsByEmployeeIdAsync(employeeId, bankTypeId, null);

            var existedPaymentTransactions = bankAccounts.Select(x => x.PaymentTransaction);
            var selectedPaymentTransaction = bankAccounts.FirstOrDefault(x => x.BankAccountId == bankAccountId)?.PaymentTransaction;

            return Json(results.Select(x => new
            {
                id = x.ResourceId,
                text = x.Value,
                disabled = x.Value != null && existedPaymentTransactions.Contains(x.Value) && isActive,
                selected = x.Value == selectedPaymentTransaction
            }));
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public async Task<IActionResult> CreateBankAccount(int employeeId, int bankTypeId = 1)
        {
            var viewModel = new CreateBankAccountViewModel
            {
                EmployeeId = employeeId,
                BankTypeId = bankTypeId
            };

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
            var viewModel = new CreateBankAccountViewModel
            {
                EmployeeId = employeeId,
                BankTypeId = bankType
            };

            await PrepareCreateBankAccountViewModel(viewModel);

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
                PaymentTransactionId = bankAccount.PaymentTransactionId,
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

            IEnumerable<ReferenceResourceModel> refResources = await refResourceService.GetReferenceResourcesByNamesAsync(refNames);
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

            IEnumerable<ReferenceResourceModel> refResources = await refResourceService.GetReferenceResourcesByNamesAsync(refNames);
            viewModel.BankTypeResources = refResourceService.FilterReferenceResources(refResources, ReferenceResource.BankType);
            viewModel.BankAccountTypeResources = refResourceService.FilterReferenceResources(refResources, ReferenceResource.TypeOfBankAccount);

            return View(viewModel);
        }

        private async Task PrepareCreateBankAccountViewModel(BaseFormBankAccountViewModel viewModel)
        {
            Employee employee = await employeeService.GetEmployeeBasicInfoByIdAsync(viewModel.EmployeeId);

            viewModel.NickName = employee.NickName;

            if (viewModel.BankTypeId == default(int))
            {
                // 1: e-Banking: Branch and Address are required.
                // 2: Direct Banking: Branch and Address are NOT required.
                viewModel.BankTypeId = 1;
            }

            IEnumerable<ReferenceResourceModel> refResources = await refResourceService.GetReferenceResourcesByNamesAsync(refNames);
            viewModel.BankTypeResources = refResourceService.FilterReferenceResources(refResources, ReferenceResource.BankType);
            viewModel.BankAccountTypeResources = refResourceService.FilterReferenceResources(refResources, ReferenceResource.TypeOfBankAccount);
            viewModel.HaveHostPermission = sessionService.HasPermissionOnCompany(employee.CompanyId, FunctionKey.EmployeeBankDetails, Policy.Host);
        }
    }
}