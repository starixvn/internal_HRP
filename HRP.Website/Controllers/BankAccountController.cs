namespace HRP.Website.Controllers
{
    using System.Linq;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Extensions;
    using Core.Filters;
    using Core.Services;
    using Criteria.Bank;
    using HRP.Models;
    using Microsoft.AspNetCore.Mvc;
    using Models.Bank;
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
                var criteria = PrepareCreateBankAccountCriteria(viewModel.BankAccount);
                var result = baseService.ExecuteNonQuery(criteria);

                return Json(new
                {
                    isSuccessful = result.IsSuccessful,
                    message = ((BankAccountError)result.Code).GetDisplayName(),
                    redirectTo = Url.Action("EmployeeBankAccounts", "BankAccount", new { viewModel.EmployeeId })
                });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        

        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult EditBankAccount(int bankAccountId)
        {
            var bankAccount = bankAccountService.GetBankAccountById(bankAccountId);

            var viewModel = new EditBankAccountViewModel
            {
                EmployeeId = bankAccount.EmployeeId,
                EmployeeName = bankAccount.FullName,
                BankAccount = new BaseFormBankAccountViewModel
                {
                    Id = bankAccount.Id,
                    AccountName = bankAccount.AccountName,
                    AccountNumber = bankAccount.AccountNumber,
                    AccountType = bankAccount.AccountType,
                    BankAddress = bankAccount.BankAddress,
                    BankBranch = bankAccount.BankBranch,
                    BankName = bankAccount.BankName,
                    BankSwiftCode = bankAccount.BankSwiftCode,
                    BankType = bankAccount.BankType,
                    BeneficiaryName = bankAccount.BeneficiaryName,
                    EmployeeId = bankAccount.EmployeeId,
                    IsActive = bankAccount.IsActive
                }
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
                var criteria = PrepareUpdateBankAccountCriteria(viewModel.BankAccount);
                var result = baseService.ExecuteNonQuery(criteria);

                return Json(new
                {
                    isSuccessful = result.IsSuccessful,
                    message = ((BankAccountError)result.Code).GetDisplayName(),
                    redirectTo = Url.Action("EmployeeBankAccounts", "BankAccount", new { viewModel.BankAccount.EmployeeId })
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
            else
            {
                var result = baseService.GetData<BankAccount>(new SearchBankAccountCriteria { EmployeeId = employeeId, BankTypeId = bankTypeId });
                return Json(result.Where(x => x.IsActive).Select(x => new { id = x.Id, text = x.AccountName }));
            }
        }

        private UpdateBankAccountCriteria PrepareUpdateBankAccountCriteria(BaseFormBankAccountViewModel bankAccount)
        {
            var criteria = new UpdateBankAccountCriteria
            {
                Id = bankAccount.Id,
                EmployeeId = bankAccount.EmployeeId,
                BankType = bankAccount.BankType,
                BankName = bankAccount.BankName,
                BankBranch = bankAccount.BankBranch,
                BankAddress = bankAccount.BankAddress,
                BankSwiftCode = bankAccount.BankSwiftCode,
                AccountName = bankAccount.AccountName,
                AccountNumber = bankAccount.AccountNumber,
                AccountType = bankAccount.AccountType,
                BeneficiaryName = bankAccount.BeneficiaryName,
                IsActive = bankAccount.IsActive
            };

            return criteria;
        }

        private CreateBankAccountCriteria PrepareCreateBankAccountCriteria(BaseFormBankAccountViewModel bankAccount)
        {
            var criteria = new CreateBankAccountCriteria
            {
                EmployeeId = bankAccount.EmployeeId,
                BankType = bankAccount.BankType,
                BankName = bankAccount.BankName,
                BankBranch = bankAccount.BankBranch,
                BankAddress = bankAccount.BankAddress,
                BankSwiftCode = bankAccount.BankSwiftCode,
                AccountName = bankAccount.AccountName,
                AccountNumber = bankAccount.AccountNumber,
                AccountType = bankAccount.AccountType,
                BeneficiaryName = bankAccount.BeneficiaryName,
                IsActive = bankAccount.IsActive
            };

            return criteria;
        }

        private void PrepareCreateBankAccountViewModel(CreateBankAccountViewModel viewModel, int selectedBankType = 0)
        {
            Employee employee = employeeService.GetEmployeeBasicInfoById(viewModel.EmployeeId);

            viewModel.EmployeeName = employee.NickName;
            viewModel.BankAccount.BankType = selectedBankType;
            var refResources = refResourceService.GetReferenceResourcesByNames(refNames);
            viewModel.BankTypeResources = refResourceService.FilterReferenceResources(refResources, ReferenceResource.BankType);
            viewModel.BankAccountTypeResources = refResourceService.FilterReferenceResources(refResources, ReferenceResource.BankAccountType);

            viewModel.HaveHostPermission = sessionService.HasPermissionOnCompany(employee.CompanyId, FunctionKey.EmployeeBankDetails, Policy.Host);
        }
    }
}