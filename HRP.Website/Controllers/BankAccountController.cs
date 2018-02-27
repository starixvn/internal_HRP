namespace HRP.Website.Controllers
{
    using System.Linq;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Filters;
    using Core.Services;
    using Criteria.BankAccount;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.BankAccount;
    using Services.Interfaces;

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
        public IActionResult EmployeeBankAccounts(EmployeeBankAccountsViewModel viewModel)
        {
            Employee employee = employeeService.GetEmployeeBasicInfoById(viewModel.EmployeeId);
            viewModel.CompanyId = employee.CompanyId;
            viewModel.EmployeeName = employee.NickName;

            viewModel.BankTypeResources = refResourceService.GetReferenceResourcesByName(ReferenceResource.BankType);
            viewModel.BankAccounts = bankAccountService.GetBankAccountsByEmployeeId(viewModel.EmployeeId, viewModel.BankTypeId, viewModel.IsActive);
            viewModel.HaveHostPermission = sessionService.HasPermissionOnCompany(employee.CompanyId, FunctionKey.EmployeeBankDetails, Policy.Host);
            viewModel.BankAccountsLimitReached =
                baseService.GetDataEntity<bool>(new CheckBankAccountLimitReachedCriteria { EmployeeId = employee.EmployeeId });

            return View(viewModel);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult GetEmployeeBankAccounts(int bankTypeId, int employeeId)
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

            var result = baseService.GetData<BankAccount>(criteria);
            return Json(result.Select(x => new { id = x.BankAccountId, text = x.AccountName }));
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult CreateBankAccount(int employeeId)
        {
            var viewModel = new CreateBankAccountViewModel { EmployeeId = employeeId };

            PrepareCreateBankAccountViewModel(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult CreateBankAccount(CreateBankAccountViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var criteria = PrepareCreateBankAccountCriteria(viewModel);
                var result = baseService.ExecuteNonQuery(criteria);

                viewModel.IsSuccessful = result.IsSuccessful;

                if (!viewModel.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            PrepareCreateBankAccountViewModel(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult CreateBankAccountInPayroll(CreateBankAccountViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var criteria = PrepareCreateBankAccountCriteria(viewModel);
                var result = baseService.ExecuteNonQuery(criteria);

                viewModel.IsSuccessful = result.IsSuccessful;

                if (!viewModel.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            PrepareCreateBankAccountViewModel(viewModel);
            return PartialView("_CreateBankAccountInPayroll", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult CreateBankAccountInPayroll(int employeeId, int bankType)
        {
            var viewModel = new CreateBankAccountViewModel { EmployeeId = employeeId };

            PrepareCreateBankAccountViewModel(viewModel, bankType);
            return PartialView("_CreateBankAccountInPayroll", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult EditBankAccount(int bankAccountId)
        {
            var bankAccount = bankAccountService.GetBankAccountById(bankAccountId);

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
        public IActionResult EditBankAccount(EditBankAccountViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                UpdateBankAccountCriteria criteria = PrepareUpdateBankAccountCriteria(viewModel);
                var result = baseService.ExecuteNonQuery(criteria);

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

        private CreateBankAccountCriteria PrepareCreateBankAccountCriteria(CreateBankAccountViewModel bankAccount)
        {
            var criteria = new CreateBankAccountCriteria
            {
                EmployeeId = bankAccount.EmployeeId,
                BankType = bankAccount.BankTypeId,
                BankName = bankAccount.BankName,
                BankBranch = bankAccount.BankBranch,
                BankAddress = bankAccount.BankAddress,
                BankSwiftCode = bankAccount.BankSwiftCode,
                AccountName = bankAccount.AccountName,
                AccountNumber = bankAccount.AccountNumber,
                AccountType = bankAccount.AccountTypeId,
                BeneficiaryName = bankAccount.BeneficiaryName,
                IsActive = bankAccount.IsActive
            };

            return criteria;
        }

        private void PrepareCreateBankAccountViewModel(BaseFormBankAccountViewModel viewModel, int selectedBankType = 1)
        {
            Employee employee = employeeService.GetEmployeeBasicInfoById(viewModel.EmployeeId);

            viewModel.NickName = employee.NickName;
            viewModel.BankTypeId = selectedBankType;

            // TODO: disable over limit bank types (or change spec)

            var refResources = refResourceService.GetReferenceResourcesByNames(refNames);
            viewModel.BankTypeResources = refResourceService.FilterReferenceResources(refResources, ReferenceResource.BankType);
            viewModel.BankAccountTypeResources = refResourceService.FilterReferenceResources(refResources, ReferenceResource.TypeOfBankAccount);

            viewModel.HaveHostPermission = sessionService.HasPermissionOnCompany(employee.CompanyId, FunctionKey.EmployeeBankDetails, Policy.Host);
        }

        private UpdateBankAccountCriteria PrepareUpdateBankAccountCriteria(EditBankAccountViewModel bankAccount)
        {
            var criteria = new UpdateBankAccountCriteria
            {
                Id = bankAccount.BankAccountId,
                EmployeeId = bankAccount.EmployeeId,
                BankType = bankAccount.BankTypeId,
                BankName = bankAccount.BankName,
                BankBranch = bankAccount.BankBranch,
                BankAddress = bankAccount.BankAddress,
                BankSwiftCode = bankAccount.BankSwiftCode,
                AccountName = bankAccount.AccountName,
                AccountNumber = bankAccount.AccountNumber,
                AccountType = bankAccount.AccountTypeId,
                BeneficiaryName = bankAccount.BeneficiaryName,
                IsActive = bankAccount.IsActive
            };

            return criteria;
        }
    }
}