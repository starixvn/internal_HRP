namespace HRP.Website.UnitTests.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Model;
    using Core.Services;
    using Criteria.BankAccount;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models.BankAccount;
    using Models.Employee;
    using NSubstitute;
    using Services;
    using Website.Controllers;
    using Xunit;

    public class BankAccountControllerTests
    {
        private IBaseService baseService;
        private IEmployeeService employeeService;
        private IBankAccountService bankAccountService;
        private ICommonService commonService;
        private ISessionService sessionService;

        [Fact]
        public async Task EmployeeBankAccounts_NoReadPermission_ReturnPermissionDenied()
        {
            // Arrange
            var controller = GetBankAccountController();
            var resources = GetReferenceResourcesForTest();

            var viewModel = new EmployeeBankAccountsViewModel
            {
                EmployeeId = 1,
                CompanyId = 1,
                IsActive = null,
                BankTypeId = null,
            };

            var employee = new EmployeeBasicInfo
            {
                CompanyId = 1,
                Nickname = "hunter.tran"
            };

            employeeService.GetEmployeeBasicInfoByIdAsync(Arg.Any<int>()).Returns(employee);
            commonService.GetReferenceResourcesByNamesAsync(Arg.Any<string[]>()).Returns(resources.Where(x => x.RefName == ReferenceResource.BankType));
            sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.EmployeeBankDetails, Policy.Read).Returns(false);

            // Test
            var actual = await controller.EmployeeBankAccounts(viewModel);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("AccessDenied", viewResult.ActionName);
        }

        [Fact]
        public async Task EmployeeBankAccounts_WhenCalled_ReturnAsExpectedViewResult()
        {
            // Arrange
            var controller = GetBankAccountController();
            var resources = GetReferenceResourcesForTest();
            var viewModel = new EmployeeBankAccountsViewModel
            {
                EmployeeId = 1,
                CompanyId = 1,
                IsActive = null,
                BankTypeId = null,
            };
            var employee = new EmployeeBasicInfo
            {
                CompanyId = 1,
                Nickname = "hunter.tran"
            };
            employeeService.GetEmployeeBasicInfoByIdAsync(Arg.Any<int>()).Returns(employee);
            commonService.GetReferenceResourcesByNamesAsync(Arg.Any<string[]>()).Returns(resources.Where(x => x.RefName == ReferenceResource.BankType));
            bankAccountService.GetBankAccountsByEmployeeIdAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>()).Returns(new List<BankAccount>());
            sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.EmployeeBankDetails, Policy.Host).Returns(true);
            sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.EmployeeBankDetails, Policy.Read).Returns(true);

            // Test
            var actual = await controller.EmployeeBankAccounts(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModelResult = Assert.IsType<EmployeeBankAccountsViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
            Assert.True(viewModelResult.HaveHostPermission);
            Assert.IsType<SelectList>(viewModelResult.BankTypes);
            Assert.NotNull(viewModelResult.BankAccounts);
            Assert.Equal(employee.Nickname, viewModelResult.EmployeeName);
        }

        #region Create Bank Account in Payroll

        [Fact]
        public async Task CreateBankAccountInPayroll_HasNotPermission_ReturnRedirectToAccessDenied()
        {
            // Arrange
            var employeeId = 1;
            var employee = new EmployeeBasicInfo
            {
                CompanyId = 1,
                Nickname = "hunter.tran"
            };
            var controller = GetBankAccountController();
            employeeService.GetEmployeeBasicInfoByIdAsync(employeeId).Returns(employee);
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), FunctionKey.EmployeeBankDetails, Policy.Host).Returns(false);

            // Test
            var actual = await controller.CreateBankAccountInPayroll(employeeId, 1);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("AccessDenied", viewResult.ActionName);
        }

        [Fact]
        public async Task CreateBankAccountInPayroll_HasPermission_ReturnAsExpectedPartialView()
        {
            // Arrange
            var employeeId = 1;
            var resources = GetReferenceResourcesForTest().OrderBy(x => x.ResourceId);
            var employee = new EmployeeBasicInfo
            {
                CompanyId = 1,
                Nickname = "hunter.tran"
            };
            var controller = GetBankAccountController();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), FunctionKey.EmployeeBankDetails, Policy.Host).Returns(true);
            employeeService.GetEmployeeBasicInfoByIdAsync(employeeId).Returns(employee);
            commonService.FilterReferenceResources(Arg.Any<IEnumerable<ReferenceResourceModel>>(), Arg.Any<string>()).Returns(resources);

            // Test
            var actual = await controller.CreateBankAccountInPayroll(employeeId, 1);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var model = Assert.IsType<CreateBankAccountViewModel>(viewResult.Model);
            Assert.Equal(model.NickName, employee.Nickname);
            Assert.Equal(resources, model.BankTypeResources);
            Assert.Equal(resources, model.BankAccountTypeResources);
            Assert.True(model.HaveHostPermission);
        }

        [Fact]
        public async Task CreateBankAccountInPayrollPost_InvalidModelState_ReturnAsExpectedPartialView()
        {
            // Arrange
            var resources = GetReferenceResourcesForTest().OrderBy(x => x.ResourceId);
            var employee = new EmployeeBasicInfo
            {
                CompanyId = 1,
                Nickname = "hunter.tran"
            };
            var viewModel = new CreateBankAccountViewModel();
            var controller = GetBankAccountController();
            employeeService.GetEmployeeBasicInfoByIdAsync(Arg.Any<int>()).Returns(employee);
            commonService.FilterReferenceResources(Arg.Any<IEnumerable<ReferenceResourceModel>>(), Arg.Any<string>()).Returns(resources);
            controller.ModelState.AddModelError("key", "message");

            // Test
            var actual = await controller.CreateBankAccountInPayroll(viewModel);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var model = Assert.IsType<CreateBankAccountViewModel>(viewResult.Model);
            Assert.Equal(model.NickName, employee.Nickname);
            Assert.Equal(resources, model.BankTypeResources);
            Assert.Equal(resources, model.BankAccountTypeResources);
        }

        [Fact]
        public async Task CreateBankAccountInPayrollPost_CreateUnsuccessfully_ReturnAsExpectedPartialView()
        {
            // Arrange
            var commandResult = new CommandResult();
            var resources = GetReferenceResourcesForTest().OrderBy(x => x.ResourceId);
            var employee = new EmployeeBasicInfo
            {
                CompanyId = 1,
                Nickname = "hunter.tran"
            };
            var viewModel = new CreateBankAccountViewModel();
            var controller = GetBankAccountController();
            employeeService.GetEmployeeBasicInfoByIdAsync(Arg.Any<int>()).Returns(employee);
            commonService.FilterReferenceResources(Arg.Any<IEnumerable<ReferenceResourceModel>>(), Arg.Any<string>()).Returns(resources);
            bankAccountService.CreateBankAccountAsync(viewModel).Returns(commandResult);

            // Test
            var actual = await controller.CreateBankAccountInPayroll(viewModel);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var model = Assert.IsType<CreateBankAccountViewModel>(viewResult.Model);
            Assert.Equal(model.NickName, employee.Nickname);
            Assert.Equal(resources, model.BankTypeResources);
            Assert.Equal(resources, model.BankAccountTypeResources);
            Assert.Equal(commandResult.IsSuccessful, viewModel.IsSuccessful);

            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.True(viewResult.ViewData.ModelState.Count == 1);
        }

        [Fact]
        public async Task CreateBankAccountInPayroll_CreateSuccessfully_ReturnAsExpectedPartialView()
        {
            // Arrange
            var resources = GetReferenceResourcesForTest().OrderBy(x => x.ResourceId);
            var employee = new EmployeeBasicInfo
            {
                CompanyId = 1,
                Nickname = "hunter.tran"
            };
            var commandResult = new CommandResult();
            var viewModel = new CreateBankAccountViewModel();
            var controller = GetBankAccountController();
            employeeService.GetEmployeeBasicInfoByIdAsync(Arg.Any<int>()).Returns(employee);
            commonService.FilterReferenceResources(Arg.Any<IEnumerable<ReferenceResourceModel>>(), Arg.Any<string>()).Returns(resources);
            bankAccountService.CreateBankAccountAsync(viewModel).Returns(commandResult);

            // Test
            var actual = await controller.CreateBankAccountInPayroll(viewModel);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var model = Assert.IsType<CreateBankAccountViewModel>(viewResult.Model);
            Assert.Equal(model.NickName, employee.Nickname);
            Assert.Equal(resources, model.BankTypeResources);
            Assert.Equal(resources, model.BankAccountTypeResources);
            Assert.Equal(commandResult.IsSuccessful, viewModel.IsSuccessful);
        }

        #endregion Create Bank Account in Payroll

        #region Create Bank Account

        [Fact]
        public async Task CreateBankAccount_HasNotPermission_ReturnRedirectToAccessDenied()
        {
            // Arrange
            var employeeId = 1;
            var employee = new EmployeeBasicInfo
            {
                CompanyId = 1,
                Nickname = "hunter.tran"
            };
            var controller = GetBankAccountController();
            employeeService.GetEmployeeBasicInfoByIdAsync(employeeId).Returns(employee);
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), FunctionKey.EmployeeBankDetails, Policy.Host).Returns(false);

            // Test
            var actual = await controller.CreateBankAccount(employeeId);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("AccessDenied", viewResult.ActionName);
        }

        [Fact]
        public async Task CreateBankAccount_HasPermission_ReturnAsExpectedViewResult()
        {
            // Arrange
            var employeeId = 1;
            var resources = GetReferenceResourcesForTest().OrderBy(x => x.ResourceId);
            var employee = new EmployeeBasicInfo
            {
                CompanyId = 1,
                Nickname = "hunter.tran"
            };
            var controller = GetBankAccountController();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), FunctionKey.EmployeeBankDetails, Policy.Host).Returns(true);
            employeeService.GetEmployeeBasicInfoByIdAsync(employeeId).Returns(employee);
            commonService.FilterReferenceResources(Arg.Any<IEnumerable<ReferenceResourceModel>>(), Arg.Any<string>()).Returns(resources);

            // Test
            var actual = await controller.CreateBankAccount(employeeId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<CreateBankAccountViewModel>(viewResult.Model);
            Assert.Equal(model.NickName, employee.Nickname);
            Assert.Equal(resources, model.BankTypeResources);
            Assert.Equal(resources, model.BankAccountTypeResources);
            Assert.True(model.HaveHostPermission);
        }

        [Fact]
        public async Task CreateBankAccountPost_InvalidModelState_ReturnAsExpectedViewResult()
        {
            // Arrange
            var resources = GetReferenceResourcesForTest().OrderBy(x => x.ResourceId);
            var employee = new EmployeeBasicInfo
            {
                CompanyId = 1,
                Nickname = "hunter.tran"
            };
            var viewModel = new CreateBankAccountViewModel();
            var controller = GetBankAccountController();
            employeeService.GetEmployeeBasicInfoByIdAsync(Arg.Any<int>()).Returns(employee);
            commonService.FilterReferenceResources(Arg.Any<IEnumerable<ReferenceResourceModel>>(), Arg.Any<string>()).Returns(resources);
            controller.ModelState.AddModelError("key", "message");

            // Test
            var actual = await controller.CreateBankAccount(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<CreateBankAccountViewModel>(viewResult.Model);
            var model = Assert.IsType<CreateBankAccountViewModel>(viewResult.Model);
            Assert.Equal(model.NickName, employee.Nickname);
            Assert.Equal(resources, model.BankTypeResources);
            Assert.Equal(resources, model.BankAccountTypeResources);
        }

        [Fact]
        public async Task CreateBankAccount_CreateUnsuccessfully_ReturnAsExpectedViewResult()
        {
            // Arrange
            var resources = GetReferenceResourcesForTest().OrderBy(x => x.ResourceId);
            var employee = new EmployeeBasicInfo
            {
                CompanyId = 1,
                Nickname = "hunter.tran"
            };
            var commandResult = new CommandResult();
            var viewModel = new CreateBankAccountViewModel();
            var controller = GetBankAccountController();
            employeeService.GetEmployeeBasicInfoByIdAsync(Arg.Any<int>()).Returns(employee);
            commonService.FilterReferenceResources(Arg.Any<IEnumerable<ReferenceResourceModel>>(), Arg.Any<string>()).Returns(resources);
            bankAccountService.CreateBankAccountAsync(viewModel).Returns(commandResult);

            // Test
            var actual = await controller.CreateBankAccount(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<CreateBankAccountViewModel>(viewResult.Model);
            Assert.Equal(model.NickName, employee.Nickname);
            Assert.Equal(resources, model.BankTypeResources);
            Assert.Equal(resources, model.BankAccountTypeResources);
            Assert.Equal(commandResult.IsSuccessful, viewModel.IsSuccessful);

            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.True(viewResult.ViewData.ModelState.Count == 1);
        }

        [Fact]
        public async Task CreateBankAccount_CreateSuccessfully_ReturnAsExpectedViewResult()
        {
            // Arrange
            var resources = GetReferenceResourcesForTest().OrderBy(x => x.ResourceId);
            var employee = new EmployeeBasicInfo
            {
                CompanyId = 1,
                Nickname = "hunter.tran"
            };
            var commandResult = new CommandResult();
            var viewModel = new CreateBankAccountViewModel();
            var controller = GetBankAccountController();
            employeeService.GetEmployeeBasicInfoByIdAsync(Arg.Any<int>()).Returns(employee);
            commonService.FilterReferenceResources(Arg.Any<IEnumerable<ReferenceResourceModel>>(), Arg.Any<string>()).Returns(resources);
            bankAccountService.CreateBankAccountAsync(viewModel).Returns(commandResult);

            // Test
            var actual = await controller.CreateBankAccount(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<CreateBankAccountViewModel>(viewResult.Model);
            Assert.Equal(model.NickName, employee.Nickname);
            Assert.Equal(resources, model.BankTypeResources);
            Assert.Equal(resources, model.BankAccountTypeResources);
            Assert.Equal(commandResult.IsSuccessful, viewModel.IsSuccessful);
        }

        #endregion Create Bank Account

        #region Edit Bank Account

        [Fact]
        public async Task EditBankAccount_HasNotPermission_ReturnRedirectToAccessDenied()
        {
            // Arrange
            var employee = new EmployeeBasicInfo
            {
                CompanyId = 1,
                Nickname = "hunter.tran"
            };
            var bankAccount = new BankAccount();
            var controller = GetBankAccountController();
            bankAccountService.GetBankAccountByIdAsync(Arg.Any<int>()).Returns(bankAccount);
            employeeService.GetEmployeeBasicInfoByIdAsync(Arg.Any<int>()).Returns(employee);
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(false);

            // Test
            var actual = await controller.EditBankAccount(1);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("AccessDenied", viewResult.ActionName);
        }

        [Fact]
        public async Task EditBankAccount_WhenCalled_ReturnAsExpectedViewResult()
        {
            // Arrange
            var controller = GetBankAccountController();
            var resources = GetReferenceResourcesForTest();
            var bankAccount = new BankAccount();
            ArrangeForEditBankAccount(bankAccount, resources);

            // Test
            var actual = await controller.EditBankAccount(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<EditBankAccountViewModel>(viewResult.Model);
            Assert.IsType<SelectList>(model.BankTypes);
            Assert.IsType<SelectList>(model.BankAccountTypes);
        }

        [Fact]
        public async Task EditBankAccount_InvalidModelState_ReturnAsExpectedViewResult()
        {
            // Arrange
            var controller = GetBankAccountController();
            var resources = GetReferenceResourcesForTest();
            var bankAccount = new BankAccount();
            ArrangeForEditBankAccount(bankAccount, resources);
            var viewModel = new EditBankAccountViewModel();
            controller.ModelState.AddModelError("key", "message");

            // Test
            var actual = await controller.EditBankAccount(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<EditBankAccountViewModel>(viewResult.Model);
            Assert.False(viewModel.IsSuccessful);
        }

        [Fact]
        public async Task EditBankAccount_EditSuccessfully_ReturnAsExpectedViewResult()
        {
            // Arrange
            var controller = GetBankAccountController();
            var resources = GetReferenceResourcesForTest();
            var bankAccount = new BankAccount();
            var result = new CommandResult
            {
                Code = 0,
            };
            var viewModel = new EditBankAccountViewModel();
            ArrangeForEditBankAccount(bankAccount, resources);
            bankAccountService.EditBankAccountAsync(viewModel).Returns(result);

            // Test
            var actual = await controller.EditBankAccount(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<EditBankAccountViewModel>(viewResult.Model);
            Assert.True(viewModel.IsSuccessful);
        }

        [Fact]
        public async Task EditBankAccount_ValidModelStateEditError_ReturnView()
        {
            // Arrange
            var controller = GetBankAccountController();
            var resources = GetReferenceResourcesForTest();
            var bankAccount = new BankAccount();
            var result = new CommandResult
            {
                Code = 1,
            };
            var viewModel = new EditBankAccountViewModel();
            ArrangeForEditBankAccount(bankAccount, resources);
            bankAccountService.EditBankAccountAsync(viewModel).Returns(result);

            // Test
            var actual = await controller.EditBankAccount(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<EditBankAccountViewModel>(viewResult.Model);
            Assert.False(viewModel.IsSuccessful);
        }

        #endregion Edit Bank Account

        #region GetEmployeeBankAccounts

        [Fact]
        public async Task GetEmployeeBankAccounts_NoReadPermission_ReturnEmployeeBankAccounts()
        {
            //Arrange
            var controller = GetBankAccountController();

            var employee = new EmployeeBasicInfo
            {
                CompanyId = 1,
                Nickname = "hunter.tran"
            };

            employeeService.GetEmployeeBasicInfoByIdAsync(Arg.Any<int>()).Returns(employee);
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), FunctionKey.EmployeeBankDetails, Policy.Read).Returns(false);
            baseService.GetDataAsync<BankAccount>(Arg.Any<SearchBankAccountCriteria>()).Returns(new List<BankAccount>());

            // Test
            var actual = await controller.GetEmployeeBankAccounts((int)BankTypes.Ebanking, 1);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("AccessDenied", viewResult.ActionName);
        }

        [Fact]
        public async Task GetEmployeeBankAccounts_ValidBankType_ReturnEmployeeBankAccounts()
        {
            //Arrange
            var controller = GetBankAccountController();

            var employee = new EmployeeBasicInfo
            {
                CompanyId = 1,
                Nickname = "hunter.tran"
            };

            employeeService.GetEmployeeBasicInfoByIdAsync(Arg.Any<int>()).Returns(employee);
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), FunctionKey.EmployeeBankDetails, Policy.Read).Returns(true);
            baseService.GetDataAsync<BankAccount>(Arg.Any<SearchBankAccountCriteria>()).Returns(new List<BankAccount>());

            // Test
            var actual = await controller.GetEmployeeBankAccounts((int)BankTypes.Ebanking, 1);

            // Assert
            Assert.IsType<JsonResult>(actual);
        }

        #endregion GetEmployeeBankAccounts

        [Fact]
        public async Task GetPaymentTransactions_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var controller = GetBankAccountController();
            var resources = GetReferenceResourcesForTest();
            commonService.FilterReferenceResources(Arg.Any<IEnumerable<ReferenceResourceModel>>(), Arg.Any<string>()).Returns(resources);

            // Test
            var actual = await controller.GetPaymentTransactions(1, 1, 1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.NotNull(jsonResult);
        }

        #region Helpers

        private void ArrangeForEditBankAccount(BankAccount bankAccount, IEnumerable<ReferenceResourceModel> resources)
        {
            bankAccountService.GetBankAccountByIdAsync(Arg.Any<int>()).Returns(bankAccount);

            commonService.GetReferenceResourcesByNamesAsync(Arg.Any<string[]>()).Returns(resources);
            commonService
                .FilterReferenceResources(Arg.Any<IEnumerable<ReferenceResourceModel>>(), Arg.Any<string>())
                .Returns(new List<ReferenceResourceModel>());

            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);

            var employee = new EmployeeBasicInfo
            {
                CompanyId = 1,
                Nickname = "hunter.tran"
            };

            employeeService.GetEmployeeBasicInfoByIdAsync(Arg.Any<int>()).Returns(employee);
        }

        private static IEnumerable<ReferenceResourceModel> GetReferenceResourcesForTest()
        {
            var resources = new List<ReferenceResourceModel>
            {
                new ReferenceResourceModel
                {
                    RefName = ReferenceResource.BankType,
                    ResourceId = (int)BankTypes.Ebanking
                },
                new ReferenceResourceModel
                {
                    RefName = ReferenceResource.BankType,
                    ResourceId = (int)BankTypes.DirectBanking
                },
                new ReferenceResourceModel
                {
                    RefName = ReferenceResource.TypeOfBankAccount,
                    ResourceId = 1
                },
                new ReferenceResourceModel
                {
                    RefName = ReferenceResource.TypeOfBankAccount,
                    ResourceId = 2
                }
            };

            return resources;
        }

        private BankAccountController GetBankAccountController()
        {
            FakeEmployeeService();
            FakeSessionService();
            FakeBaseService();
            FakeBankAccountService();
            FakeCommonService();

            return new BankAccountController(baseService, employeeService, bankAccountService, commonService, sessionService);
        }

        private void FakeEmployeeService()
        {
            employeeService = Substitute.For<IEmployeeService>();
        }

        private void FakeSessionService()
        {
            sessionService = Substitute.For<ISessionService>();
        }

        private void FakeBaseService()
        {
            baseService = Substitute.For<IBaseService>();
        }

        private void FakeBankAccountService()
        {
            bankAccountService = Substitute.For<IBankAccountService>();
        }

        private void FakeCommonService()
        {
            commonService = Substitute.For<ICommonService>();
        }

        #endregion Helpers
    }
}