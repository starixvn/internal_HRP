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
        public async Task<IActionResult> EditEmployeeBankDetails(EditEmployeeBankDetailsViewModel viewModel)
        {
            var employee = employeeManagementService.GetEmployeeById(viewModel.EmployeeId);

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

            var bankAccounts = bankAccountService.GetBankAccountsByEmployeeId(viewModel.EmployeeId);
            viewModel.BankAccounts = bankAccountService
                .FilterBankAccounts(bankAccounts, viewModel.SelectedBankTypes, viewModel.IsActiveBankAccount)
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
                var redirectViewModel = new EditEmployeeBankDetailsViewModel { EmployeeId = viewModel.BankAccount.EmployeeId };
                return RedirectToAction("EditEmployeeBankDetails", redirectViewModel);
            }

            return View(viewModel);
        }

        private static IEnumerable<ReferenceResourceModel> GetRefResourceByName(IEnumerable<ReferenceResourceModel> refResources, string refName)
        {
            return refResources.Where(x => x.RefName == refName);
        }
    }
}