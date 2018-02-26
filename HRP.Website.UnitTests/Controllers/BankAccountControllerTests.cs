namespace HRP.Website.UnitTests.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Model;
    using Core.Services;
    using Criteria.BankAccount;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.BankAccount;
    using NSubstitute;
    using Services.Interfaces;
    using Website.Controllers;
    using Xunit;

    public class BankAccountControllerTests
    {
        private IBaseService baseService;
        private IEmployeeService employeeService;
        private IBankAccountService bankAccountService;
        private IRefResourceService refResourceService;
        private ISessionService sessionService;

        [Fact]
        public void EmployeeBankAccounts_WhenCalled_ReturnAsExpectedViewResult()
        {
            // Arrange
            var controller = GetBankAccountController();
            var resources = GetResources();

            var viewModel = new EmployeeBankAccountsViewModel
            {
                EmployeeId = 1,
                CompanyId = 1,
            };

            var employee = new Employee
            {
                CompanyId = 1,
                NickName = "hunter.tran"
            };

            employeeService.GetEmployeeBasicInfoById(Arg.Any<int>()).Returns(employee);
            refResourceService.GetReferenceResourcesByName(Arg.Any<string>()).Returns(resources.Where(x => x.RefName == ReferenceResource.BankType));
            bankAccountService.GetBankAccountsByEmployeeId(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>()).Returns(new List<BankAccount>());
            sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.EmployeeBankDetails, Policy.Host).Returns(true);
            baseService.GetDataEntity<bool>(Arg.Any<CheckBankAccountLimitReachedCriteria>()).Returns(true);

            // Test
            var actual = controller.EmployeeBankAccounts(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<EmployeeBankAccountsViewModel>(viewResult.Model);
            Assert.True(model.HaveHostPermission);
            Assert.True(model.BankAccountsLimitReached);
        }

        #region Create Bank Account in Payroll

        [Fact]
        public void CreateBankAccountInPayroll_WhenCalled_ReturnPartialView()
        {
            // Arrange
            var controller = GetBankAccountController();
            var resources = GetResources();

            var employee = new Employee
            {
                CompanyId = 1,
                NickName = "hunter.tran"
            };

            ArrangeForCreateBankAccount(employee, resources);

            // Test
            var actual = controller.CreateBankAccountInPayroll(1, 1);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var model = Assert.IsType<CreateBankAccountViewModel>(viewResult.Model);
            Assert.True(model.HaveHostPermission);
        }

        [Fact]
        public void CreateBankAccountInPayroll_InValidModelState_ReturnPartialView()
        {
            // Arrange
            var controller = GetBankAccountController();
            var resources = GetResources();

            var employee = new Employee
            {
                CompanyId = 1,
                NickName = "hunter.tran"
            };

            var viewModel = new CreateBankAccountViewModel();

            ArrangeForCreateBankAccount(employee, resources);
            controller.ModelState.AddModelError("key", "message");

            // Test
            var actual = controller.CreateBankAccountInPayroll(viewModel);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            Assert.IsType<CreateBankAccountViewModel>(viewResult.Model);
            Assert.False(viewModel.IsSuccessful);
        }

        [Fact]
        public void CreateBankAccountInPayroll_ValidModelStateCreateError_ReturnPartialView()
        {
            // Arrange
            var controller = GetBankAccountController();
            var resources = GetResources();

            var employee = new Employee
            {
                CompanyId = 1,
                NickName = "hunter.tran"
            };

            var result = new CommandResult
            {
                Code = 0,
            };

            var viewModel = new CreateBankAccountViewModel();

            ArrangeForCreateBankAccount(employee, resources);

            baseService.ExecuteNonQuery(Arg.Any<CreateBankAccountCriteria>()).Returns(result);

            // Test
            var actual = controller.CreateBankAccountInPayroll(viewModel);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            Assert.IsType<CreateBankAccountViewModel>(viewResult.Model);
            Assert.True(viewModel.IsSuccessful);
        }

        [Fact]
        public void CreateBankAccountInPayroll_ValidModelStateCreateSuccessful_ReturnPartialView()
        {
            // Arrange
            var controller = GetBankAccountController();
            var resources = GetResources();

            var employee = new Employee
            {
                CompanyId = 1,
                NickName = "hunter.tran"
            };

            var result = new CommandResult
            {
                Code = 1,
            };

            var viewModel = new CreateBankAccountViewModel();

            ArrangeForCreateBankAccount(employee, resources);

            baseService.ExecuteNonQuery(Arg.Any<CreateBankAccountCriteria>()).Returns(result);

            // Test
            var actual = controller.CreateBankAccountInPayroll(viewModel);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            Assert.IsType<CreateBankAccountViewModel>(viewResult.Model);
            Assert.False(viewModel.IsSuccessful);
        }

        #endregion

        #region Create Bank Account

        [Fact]
        public void CreateBankAccount_WhenCalled_ReturnView()
        {
            // Arrange
            var controller = GetBankAccountController();
            var resources = GetResources();

            var employee = new Employee
            {
                CompanyId = 1,
                NickName = "hunter.tran"
            };

            ArrangeForCreateBankAccount(employee, resources);

            // Test
            var actual = controller.CreateBankAccount(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<CreateBankAccountViewModel>(viewResult.Model);
            Assert.True(model.HaveHostPermission);
        }

        [Fact]
        public void CreateBankAccount_InValidModelState_ReturnView()
        {
            // Arrange
            var controller = GetBankAccountController();
            var resources = GetResources();

            var employee = new Employee
            {
                CompanyId = 1,
                NickName = "hunter.tran"
            };

            var viewModel = new CreateBankAccountViewModel();

            ArrangeForCreateBankAccount(employee, resources);
            controller.ModelState.AddModelError("key", "message");

            // Test
            var actual = controller.CreateBankAccount(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<CreateBankAccountViewModel>(viewResult.Model);
            Assert.False(viewModel.IsSuccessful);
        }

        [Fact]
        public void CreateBankAccount_ValidModelStateCreateError_ReturnView()
        {
            // Arrange
            var controller = GetBankAccountController();
            var resources = GetResources();

            var employee = new Employee
            {
                CompanyId = 1,
                NickName = "hunter.tran"
            };

            var result = new CommandResult
            {
                Code = 0,
            };

            var viewModel = new CreateBankAccountViewModel();

            ArrangeForCreateBankAccount(employee, resources);

            baseService.ExecuteNonQuery(Arg.Any<CreateBankAccountCriteria>()).Returns(result);

            // Test
            var actual = controller.CreateBankAccount(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<CreateBankAccountViewModel>(viewResult.Model);
            Assert.True(viewModel.IsSuccessful);
        }

        [Fact]
        public void CreateBankAccount_ValidModelStateCreateSuccessful_ReturnView()
        {
            // Arrange
            var controller = GetBankAccountController();
            var resources = GetResources();

            var employee = new Employee
            {
                CompanyId = 1,
                NickName = "hunter.tran"
            };

            var result = new CommandResult
            {
                Code = 1,
            };

            var viewModel = new CreateBankAccountViewModel();

            ArrangeForCreateBankAccount(employee, resources);

            baseService.ExecuteNonQuery(Arg.Any<CreateBankAccountCriteria>()).Returns(result);

            // Test
            var actual = controller.CreateBankAccount(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<CreateBankAccountViewModel>(viewResult.Model);
            Assert.False(viewModel.IsSuccessful);
        }

        #endregion

        #region Edit Bank Account

        [Fact]
        public void EditBankAccount_WhenCalled_ReturnView()
        {
            // Arrange
            var controller = GetBankAccountController();
            var resources = GetResources();

            var bankAccount = new BankAccount();

            ArrangeForEditBankAccount(bankAccount, resources);

            // Test
            var actual = controller.EditBankAccount(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<EditBankAccountViewModel>(viewResult.Model);
        }

        [Fact]
        public void EditBankAccount_InValidModelState_ReturnView()
        {
            // Arrange
            var controller = GetBankAccountController();
            var resources = GetResources();

            var bankAccount = new BankAccount();

            ArrangeForEditBankAccount(bankAccount, resources);

            var viewModel = new EditBankAccountViewModel();
            controller.ModelState.AddModelError("key", "message");

            // Test
            var actual = controller.EditBankAccount(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<EditBankAccountViewModel>(viewResult.Model);
            Assert.False(viewModel.IsSuccessful);
        }

        [Fact]
        public void EditBankAccount_ValidModelStateEditSuccessful_ReturnView()
        {
            // Arrange
            var controller = GetBankAccountController();
            var resources = GetResources();

            var bankAccount = new BankAccount();

            var result = new CommandResult
            {
                Code = 0,
            };

            var viewModel = new EditBankAccountViewModel();

            ArrangeForEditBankAccount(bankAccount, resources);

            baseService.ExecuteNonQuery(Arg.Any<UpdateBankAccountCriteria>()).Returns(result);

            // Test
            var actual = controller.EditBankAccount(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<EditBankAccountViewModel>(viewResult.Model);
            Assert.True(viewModel.IsSuccessful);
        }

        [Fact]
        public void EditBankAccount_ValidModelStateEditError_ReturnView()
        {
            // Arrange
            var controller = GetBankAccountController();
            var resources = GetResources();

            var bankAccount = new BankAccount();

            var result = new CommandResult
            {
                Code = 1,
            };

            var viewModel = new EditBankAccountViewModel();

            ArrangeForEditBankAccount(bankAccount, resources);

            baseService.ExecuteNonQuery(Arg.Any<UpdateBankAccountCriteria>()).Returns(result);

            // Test
            var actual = controller.EditBankAccount(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<EditBankAccountViewModel>(viewResult.Model);
            Assert.False(viewModel.IsSuccessful);
        }

        #endregion

        [Fact]
        public void GetEmployeeBankAccounts_ContraCoAccountType_ReturnContraCoAccounts()
        {
            // Arrange
            var controller = GetBankAccountController();

            refResourceService.GetReferenceResourcesByName(ReferenceResource.ContraCoAccount)
                .Returns(new List<ReferenceResourceModel>());

            // Test
            var actual = controller.GetEmployeeBankAccounts((int)BankTypes.ContraCoAccount, 1);

            // Assert
            Assert.IsType<JsonResult>(actual);
        }

        [Fact]
        public void GetEmployeeBankAccounts_NotContraCoAccountType_ReturnEmployeeBankAccounts()
        {
            //Arrange
            var controller = GetBankAccountController();

            baseService.GetData<BankAccount>(Arg.Any<SearchBankAccountCriteria>()).Returns(new List<BankAccount>());

            // Test
            var actual = controller.GetEmployeeBankAccounts((int)BankTypes.Ebanking, 1);

            // Assert
            Assert.IsType<JsonResult>(actual);
        }

        #region Helpers

        private void ArrangeForCreateBankAccount(Employee employee, IEnumerable<ReferenceResourceModel> resources)
        {
            employeeService.GetEmployeeBasicInfoById(Arg.Any<int>()).Returns(employee);

            refResourceService.GetReferenceResourcesByNames(Arg.Any<string[]>()).Returns(resources);
            refResourceService
                .FilterReferenceResources(Arg.Any<IEnumerable<ReferenceResourceModel>>(), Arg.Any<string>())
                .Returns(new List<ReferenceResourceModel>());
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), FunctionKey.EmployeeBankDetails, Policy.Host).Returns(true);
        }

        private void ArrangeForEditBankAccount(BankAccount bankAccount, IEnumerable<ReferenceResourceModel> resources)
        {
            bankAccountService.GetBankAccountById(Arg.Any<int>()).Returns(bankAccount);

            refResourceService.GetReferenceResourcesByNames(Arg.Any<string[]>()).Returns(resources);
            refResourceService
                .FilterReferenceResources(Arg.Any<IEnumerable<ReferenceResourceModel>>(), Arg.Any<string>())
                .Returns(new List<ReferenceResourceModel>());
        }

        private static IEnumerable<ReferenceResourceModel> GetResources()
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
                },
                new ReferenceResourceModel
                {
                    RefName = ReferenceResource.ContraCoAccount,
                    ResourceId = 1
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
            FakeRefResourceService();

            return new BankAccountController(baseService, employeeService, bankAccountService, refResourceService, sessionService);
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

        private void FakeRefResourceService()
        {
            refResourceService = Substitute.For<IRefResourceService>();
        }

        #endregion
    }
}