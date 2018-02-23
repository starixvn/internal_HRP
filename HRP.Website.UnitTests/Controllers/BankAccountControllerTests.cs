namespace HRP.Website.UnitTests.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Constants;
    using Core.Enumerations;
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
            refResourceService.GetReferenceResourcesByName(ReferenceResource.BankType).Returns(resources.Where(x => x.RefName == ReferenceResource.BankType));
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

        [Fact]
        public void GetEmployeeBankAccounts_ContraCoAccountType_ReturnContraCoAccounts()
        {
            var controller = GetBankAccountController();
            var refNames = new[] { ReferenceResource.ContraCoAccount };

            refResourceService.GetReferenceResourcesByNames(refNames).Returns(new List<ReferenceResourceModel>());

            var actual = controller.GetEmployeeBankAccounts((int)BankTypes.ContraCoAccount, 1);

            baseService.DidNotReceive().GetData<BankAccount>(Arg.Any<SearchBankAccountCriteria>());
            refResourceService.Received().GetReferenceResourcesByNames(Arg.Any<string[]>());
            Assert.IsType<JsonResult>(actual);
        }

        [Fact]
        public void GetEmployeeBankAccounts_NotContraCoAccountType_ReturnEmployeeBankAccounts()
        {
            var controller = GetBankAccountController();

            baseService.GetData<BankAccount>(Arg.Any<SearchBankAccountCriteria>()).Returns(new List<BankAccount>());

            var actual = controller.GetEmployeeBankAccounts((int)BankTypes.Ebanking, 1);

            baseService.Received().GetData<BankAccount>(Arg.Any<SearchBankAccountCriteria>());
            refResourceService.DidNotReceive().GetReferenceResourcesByNames(Arg.Any<string[]>());
            Assert.IsType<JsonResult>(actual);
        }

        private static IEnumerable<ReferenceResourceModel> GetResources()
        {
            var resources = new List<ReferenceResourceModel>
                {
                    new ReferenceResourceModel
                    {
                        RefName = ReferenceResource.BankType,
                        ResourceId = (int) BankTypes.Ebanking
                    },
                    new ReferenceResourceModel
                    {
                        RefName = ReferenceResource.BankType,
                        ResourceId = (int) BankTypes.DirectBanking
                    },
                    new ReferenceResourceModel
                    {
                        RefName = ReferenceResource.BankAccountType,
                        ResourceId = 1
                    },
                    new ReferenceResourceModel
                    {
                        RefName = ReferenceResource.BankAccountType,
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
    }
}