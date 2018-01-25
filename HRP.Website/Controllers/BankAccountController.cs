namespace HRP.Website.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Filters;
    using Core.Services;
    using Criteria;
    using HRP.Criteria.Bank;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Bank;
    using Services;
    using Services.Interfaces;

    [HRPAuthorize]
    public class BankAccountController : Controller
    {
        private readonly IBaseService baseService;
        private readonly IEmployeeManagementService employeeManagementService;
        private readonly IBankAccountService bankAccountService;

        public BankAccountController(
            IBaseService baseService,
            IEmployeeManagementService employeeManagementService,
            IBankAccountService bankAccountService)
        {
            this.baseService = baseService;
            this.employeeManagementService = employeeManagementService;
            this.bankAccountService = bankAccountService;
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Read)]
        public async Task<IActionResult> EmployeeBankAccounts(EditEmployeeBankDetailsViewModel viewModel)
        {
            var employee = employeeManagementService.GetEmployeeById(viewModel.Id);

            var refNames = string.Format(
                "{0},{1}",
                ReferenceResource.BankType,
                ReferenceResource.BankAccountType);

            var refResources =
                await baseService.GetDataAsync<ReferenceResourceModel>(
                    new GetRefResourceByNamesCriteria { RefNames = refNames });

            var bankTypes = GetRefResourceByName(refResources, ReferenceResource.BankType);
            var bankAccountTypes = GetRefResourceByName(refResources, ReferenceResource.BankAccountType);

            viewModel.Initialize(employee.FullName, bankTypes, bankAccountTypes);

            var bankAccounts = bankAccountService.GetBankAccountsByEmployeeId(viewModel.Id);

            if(viewModel.SelectedBankTypes != null && !viewModel.SelectedBankTypes.Any())
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
        public async Task<IActionResult> CreateNewBankAccount(int employeeId)
        {
            var employee = employeeManagementService.GetEmployeeById(employeeId);

            var refNames = string.Format(
                "{0},{1}",
                ReferenceResource.BankType,
                ReferenceResource.BankAccountType);

            var refResources =
                await baseService.GetDataAsync<ReferenceResourceModel>(
                    new GetRefResourceByNamesCriteria { RefNames = refNames });

            var bankTypes = GetRefResourceByName(refResources, ReferenceResource.BankType);
            var bankAccountTypes = GetRefResourceByName(refResources, ReferenceResource.BankAccountType);

            var viewModel = new CreateNewBankAccountViewModel
            {
                EmployeeId = employeeId,
                EmployeeName = employee.FullName
            };

            viewModel.Initialize(employee.FullName, bankTypes, bankAccountTypes);

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public IActionResult CreateNewBankAccount(CreateNewBankAccountViewModel viewModel)
        {
            var result = bankAccountService.CreateBankAccount(viewModel.BankAccount);

            if (result.IsSuccessful)
            {
                var redirectViewModel = new EditEmployeeBankDetailsViewModel { Id = viewModel.BankAccount.EmployeeId };
                return RedirectToAction("EmployeeBankAccounts", redirectViewModel);
            }

            return View(viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.EmployeeBankDetails, Policy = Policy.Host)]
        public async Task<IActionResult> EditBankAccount(int bankAccountId)
        {
            var bankAccount = bankAccountService.GetBankAccountById(bankAccountId);

            var employee = employeeManagementService.GetEmployeeById(bankAccount.EmployeeId);

            var refNames = string.Format(
                "{0},{1}",
                ReferenceResource.BankType,
                ReferenceResource.BankAccountType);

            var refResources =
                await baseService.GetDataAsync<ReferenceResourceModel>(
                    new GetRefResourceByNamesCriteria { RefNames = refNames });

            var bankTypes = GetRefResourceByName(refResources, ReferenceResource.BankType);
            var bankAccountTypes = GetRefResourceByName(refResources, ReferenceResource.BankAccountType);

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
                var redirectViewModel = new EditEmployeeBankDetailsViewModel { Id = viewModel.BankAccount.EmployeeId };
                return RedirectToAction("EmployeeBankAccounts", redirectViewModel);
            }

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult GetEmployeeBankAccounts(int bankTypeId, int employeeId)
        {
            if (bankTypeId == (int) Core.Enumerations.BankType.ContraCoAccount)
            {
                var result = baseService.GetData<ReferenceResourceModel>(new GetRefResourceByNamesCriteria { RefNames = ReferenceResource.ContraCoAccount });
                return Json(result.Select(x => new { id = x.ResourceId, text = x.Value }));
            }
            else
            {
                var result = baseService.GetData<BankAccount>(new SearchBankAccountCriteria { EmployeeId = employeeId, BankTypeId = bankTypeId });
                return Json(result.Select(x => new { id = x.Id, text = x.AccountName }));
            }
        }

        private static IEnumerable<ReferenceResourceModel> GetRefResourceByName(IEnumerable<ReferenceResourceModel> refResources, string refName)
        {
            return refResources.Where(x => x.RefName == refName);
        }
    }
}