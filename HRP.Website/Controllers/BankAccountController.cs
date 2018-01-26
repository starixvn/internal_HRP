namespace HRP.Website.Controllers
{
    using System.Linq;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Filters;
    using Core.Services;
    using Criteria.Bank;
    using Microsoft.AspNetCore.Mvc;
    using Models.Bank;
    using Services.Interfaces;

    [HRPAuthorize]
    public class BankAccountController : Controller
    {
        private readonly IBaseService baseService;
        private readonly IEmployeeManagementService employeeManagementService;
        private readonly IBankAccountService bankAccountService;
        private readonly IRefResourceService refResourceService;

        private readonly string[] refNames =
        {
            ReferenceResource.BankType,
            ReferenceResource.BankAccountType,
            ReferenceResource.ContraCoAccount
        };

        public BankAccountController(
            IBaseService baseService,
            IEmployeeManagementService employeeManagementService,
            IBankAccountService bankAccountService,
            IRefResourceService refResourceService)
        {
            this.baseService = baseService;
            this.employeeManagementService = employeeManagementService;
            this.bankAccountService = bankAccountService;
            this.refResourceService = refResourceService;
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Read)]
        public IActionResult EmployeeBankAccounts(EmployeeBankAccountsViewModel viewModel)
        {
            var employee = employeeManagementService.GetEmployeeById(viewModel.Id);

            var refResources = refResourceService.GetReferenceResourcesByNames(refNames);

            var bankTypes = refResourceService.FilterReferenceResources(refResources, ReferenceResource.BankType);
            var bankAccountTypes = refResourceService.FilterReferenceResources(refResources, ReferenceResource.BankAccountType);

            viewModel.Initialize(employee.FullName, bankTypes, bankAccountTypes);

            var bankAccounts = bankAccountService.GetBankAccountsByEmployeeId(viewModel.Id);

            if ((viewModel.SelectedBankTypes != null && !viewModel.SelectedBankTypes.Any())
                || viewModel.SelectedBankTypes == null)
            {
                viewModel.SelectedBankTypes = bankTypes.Select(x => x.ResourceId).ToArray();
            }

            viewModel.BankAccounts = bankAccountService.FilterBankAccounts(
                    bankAccounts,
                    viewModel.SelectedBankTypes,
                    viewModel.IsActiveBankAccount)
                .ToList();

            return View(viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult CreateBankAccount(int employeeId)
        {
            var employee = employeeManagementService.GetEmployeeById(employeeId);

            var refResources = refResourceService.GetReferenceResourcesByNames(refNames);

            var bankTypes = refResourceService.FilterReferenceResources(refResources, ReferenceResource.BankType);
            var bankAccountTypes = refResourceService.FilterReferenceResources(refResources, ReferenceResource.BankAccountType);

            var viewModel = new CreateBankAccountViewModel
            {
                EmployeeId = employeeId,
                EmployeeName = employee.FullName
            };

            viewModel.Initialize(employee.FullName, bankTypes, bankAccountTypes);

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult CreateBankAccount(CreateBankAccountViewModel viewModel)
        {
            var result = bankAccountService.CreateBankAccount(viewModel.BankAccount);

            if (result.IsSuccessful)
            {
                var redirectViewModel = new EmployeeBankAccountsViewModel { Id = viewModel.BankAccount.EmployeeId };
                return RedirectToAction("EmployeeBankAccounts", redirectViewModel);
            }

            return View(viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult EditBankAccount(int bankAccountId)
        {
            var bankAccount = bankAccountService.GetBankAccountById(bankAccountId);

            var employee = employeeManagementService.GetEmployeeById(bankAccount.EmployeeId);

            var refResources = refResourceService.GetReferenceResourcesByNames(refNames);

            var bankTypes = refResourceService.FilterReferenceResources(refResources, ReferenceResource.BankType);
            var bankAccountTypes = refResourceService.FilterReferenceResources(refResources, ReferenceResource.BankAccountType);

            var viewModel = new EditBankAccountViewModel
            {
                EmployeeId = bankAccount.EmployeeId,
                EmployeeName = employee.FullName
            };

            viewModel.Initialize(employee.FullName, bankAccount, bankTypes, bankAccountTypes);

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult EditBankAccount(EditBankAccountViewModel viewModel)
        {
            var result = bankAccountService.UpdateBankAccount(viewModel.BankAccount);

            if (result.IsSuccessful)
            {
                var redirectViewModel = new EmployeeBankAccountsViewModel { Id = viewModel.BankAccount.EmployeeId };
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