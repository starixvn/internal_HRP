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
        private readonly IEmployeeService employeeService;
        private readonly IBankAccountService bankAccountService;
        private readonly IRefResourceService refResourceService;

        private readonly string[] refNames =
        {
            ReferenceResource.BankType,
            ReferenceResource.BankAccountType
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
            return PartialView("_CreateBankAccountInPayroll", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult CreateBankAccountInPayroll(CreateBankAccountViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var criteria = PrepareCreateBankAccountCriteria(viewModel);
                var result = baseService.ExecuteNonQuery(criteria);

                return Json(new
                {
                    isSuccessful = result.IsSuccessful,
                    message = result.Message,
                    redirectTo = Url.Action("EmployeeBankAccounts", "BankAccount", new { viewModel.EmployeeId })
                });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult CreateBankAccount(CreateBankAccountViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var criteria = PrepareCreateBankAccountCriteria(viewModel);
                var result = baseService.ExecuteNonQuery(criteria);

                if (result.IsSuccessful)
                {
                    viewModel.IsSuccessful = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            PrepareCreateBankAccountViewModel(viewModel);
            return View(viewModel);
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
            viewModel.BankAccountTypeResources = refResourceService.FilterReferenceResources(refResources, ReferenceResource.BankAccountType);

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult EditBankAccount(EditBankAccountViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var criteria = PrepareUpdateBankAccountCriteria(viewModel);
                var result = baseService.ExecuteNonQuery(criteria);

                return Json(new
                {
                    isSuccessful = result.IsSuccessful,
                    message = result.Message,
                    redirectTo = Url.Action("EmployeeBankAccounts", "BankAccount", new { viewModel.EmployeeId })
                });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });

            return Json(new { isSuccessful = false, errors });
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult GetEmployeeBankAccounts(int bankTypeId, int employeeId)
        {
            if (bankTypeId == (int)BankTypes.ContraCoAccount)
            {
                var refResources = refResourceService.GetReferenceResourcesByNames(new[] { ReferenceResource.ContraCoAccount });
                return Json(refResources.Select(x => new { id = x.ResourceId, text = x.Value }));
            }

            var result = baseService.GetData<BankAccount>(new SearchBankAccountCriteria { EmployeeId = employeeId, BankTypeId = bankTypeId });
            return Json(result.Where(x => x.IsActive).Select(x => new { id = x.BankAccountId, text = x.AccountName }));
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

        private void PrepareCreateBankAccountViewModel(BaseFormBankAccountViewModel viewModel, int selectedBankType = 0)
        {
            Employee employee = employeeService.GetEmployeeBasicInfoById(viewModel.EmployeeId);

            viewModel.NickName = employee.NickName;
            viewModel.BankTypeId = selectedBankType;
            var refResources = refResourceService.GetReferenceResourcesByNames(refNames);
            viewModel.BankTypeResources = refResourceService.FilterReferenceResources(refResources, ReferenceResource.BankType);
            viewModel.BankAccountTypeResources = refResourceService.FilterReferenceResources(refResources, ReferenceResource.BankAccountType);

            viewModel.HaveHostPermission = sessionService.HasPermissionOnCompany(employee.CompanyId, FunctionKey.EmployeeBankDetails, Policy.Host);
        }
    }
}