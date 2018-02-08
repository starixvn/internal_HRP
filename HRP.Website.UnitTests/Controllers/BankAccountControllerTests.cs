namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Extensions;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.Bank;
    using HRP.Models;
    using HRP.Models.Bank;
    using HRP.Services.Interfaces;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;

    public class BankAccountControllerTests
    {
        private IBaseService baseService;
        private IBankAccountService bankAccountService;
        private IRefResourceService refResourceService;
        private ISessionService sessionService;

        [Fact]
        public void EmployeeBankAccounts_NoExistingAccount_ReturnViewAndCanCreate()
        {
            var controller = GetBankAccountController();
            var resources = GetResources();
            var viewModel = new EmployeeBankAccountsViewModel
            {
                EmployeeId = 1,
                CompanyId = 1,
            };
            var employeeBankAcount = new EmployeeBankAccounts
            {
                EmployeeId = viewModel.EmployeeId,
                CompanyId = viewModel.CompanyId,
                BankAccounts = new List<BankAccount>()
            };

            bankAccountService.GetBankAccountsByEmployeeId(viewModel.EmployeeId).Returns(employeeBankAcount);
            refResourceService.GetReferenceResourcesByNames(Arg.Any<string[]>()).Returns(resources);
            refResourceService.FilterReferenceResources(resources, ReferenceResource.BankType).Returns(resources.Where(x => x.RefName == ReferenceResource.BankType));
            refResourceService.FilterReferenceResources(resources, ReferenceResource.BankAccountType).Returns(resources.Where(x => x.RefName == ReferenceResource.BankAccountType));

            sessionService.HasPermissionOnFunction(FunctionKey.EmployeeBankDetails, Policy.Host).Returns(true);
            sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.EmployeeBankDetails, Policy.Host).Returns(true);
            bankAccountService.FilterBankAccounts(Arg.Any<IEnumerable<BankAccount>>(), Arg.Any<int>(), Arg.Any<bool>()).Returns(employeeBankAcount.BankAccounts);

            var actual = controller.EmployeeBankAccounts(viewModel);
            var viewResult = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<EmployeeBankAccountsViewModel>(viewResult.Model);
            Assert.True(model.CanCreate);
        }

        [Fact]
        public void EmployeeBankAccounts_MaximumExistingAccount_ReturnViewAndCanNotCreate()
        {
            var controller = GetBankAccountController();
            var resources = GetResources();
            var viewModel = new EmployeeBankAccountsViewModel
            {
                EmployeeId = 1,
                CompanyId = 1,
            };
            var employeeBankAcount = new EmployeeBankAccounts
            {
                EmployeeId = viewModel.EmployeeId,
                CompanyId = viewModel.CompanyId,
                BankAccounts = new List<BankAccount>
                {
                    new BankAccount { BankType = 1, IsActive = true },
                    new BankAccount { BankType = 1, IsActive = true },
                    new BankAccount { BankType = 1, IsActive = true },
                    new BankAccount { BankType = 2, IsActive = true },
                    new BankAccount { BankType = 2, IsActive = true },
                    new BankAccount { BankType = 2, IsActive = true },
                }
            };

            bankAccountService.GetBankAccountsByEmployeeId(viewModel.EmployeeId).Returns(employeeBankAcount);
            refResourceService.GetReferenceResourcesByNames(Arg.Any<string[]>()).Returns(resources);
            refResourceService.FilterReferenceResources(resources, ReferenceResource.BankType).Returns(resources.Where(x => x.RefName == ReferenceResource.BankType));
            refResourceService.FilterReferenceResources(resources, ReferenceResource.BankAccountType).Returns(resources.Where(x => x.RefName == ReferenceResource.BankAccountType));

            sessionService.HasPermissionOnFunction(FunctionKey.EmployeeBankDetails, Policy.Host).Returns(true);
            sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.EmployeeBankDetails, Policy.Host).Returns(true);
            bankAccountService.FilterBankAccounts(Arg.Any<IEnumerable<BankAccount>>(), Arg.Any<int>(), Arg.Any<bool>()).Returns(employeeBankAcount.BankAccounts);

            var actual = controller.EmployeeBankAccounts(viewModel);
            var viewResult = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<EmployeeBankAccountsViewModel>(viewResult.Model);
            Assert.False(model.CanCreate);
        }

        [Fact]
        public void CreateBankAccount_ValidEmployeeId_ReturnView()
        {
            var controller = GetBankAccountController();
            var resources = GetResources();
            var employeeId = 1;
            var employeeBankAcount = new EmployeeBankAccounts
            {
                EmployeeId = employeeId,
                CompanyId = 1,
                BankAccounts = new List<BankAccount>()
            };

            bankAccountService.GetBankAccountsByEmployeeId(employeeId).Returns(employeeBankAcount);
            refResourceService.GetReferenceResourcesByNames(Arg.Any<string[]>()).Returns(resources);
            refResourceService.FilterReferenceResources(resources, ReferenceResource.BankType).Returns(resources.Where(x => x.RefName == ReferenceResource.BankType));
            refResourceService.FilterReferenceResources(resources, ReferenceResource.BankAccountType).Returns(resources.Where(x => x.RefName == ReferenceResource.BankAccountType));

            var actual = controller.CreateBankAccount(employeeId);
            var viewResult = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<CreateBankAccountViewModel>(viewResult.Model);
        }

        [Fact]
        public void CreateBankAccount_ValidModel_ReturnJsonWithSuccess()
        {
            var controller = GetBankAccountController();
            var employeeId = 1;
            var model = new CreateBankAccountViewModel
            {
                EmployeeId = employeeId,
                BankAccount = new BankAccount { EmployeeId = employeeId }
            };
            bankAccountService.CreateBankAccount(model.BankAccount).Returns(new CommandResult { Code = 0 });
            controller.Url = Substitute.For<IUrlHelper>();
            controller.Url.Action().ReturnsForAnyArgs("/EmployeeBankAccounts/BankAccount");

            var actual = controller.CreateBankAccount(model);

            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.True((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public void CreateBankAccount_InValidModel_ReturnJsonWithError()
        {
            var controller = GetBankAccountController();
            var employeeId = 1;
            var model = new CreateBankAccountViewModel
            {
                EmployeeId = employeeId,
                BankAccount = new BankAccount { EmployeeId = employeeId }
            };

            controller.ModelState.AddModelError(nameof(model.BankAccount.AccountName), "Error");
            var actual = controller.CreateBankAccount(model);

            bankAccountService.DidNotReceive().CreateBankAccount(model.BankAccount);
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public void CheckAccountNameExisted_AccountExisted_ReturnJsonWithMessage()
        {
            var bankAccount = new BankAccount();
            var controller = GetBankAccountController();
            bankAccountService.CheckIsAccountNameExisted(bankAccount).Returns(true);

            var actual = controller.CheckAccountNameExisted(bankAccount);
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(BankAccountError.AccountNameExisted.GetDisplayName(), jsonResult.Value.ToString());
        }

        [Fact]
        public void CheckAccountNameExisted_AccountNotExisted_ReturnJsonWithTrue()
        {
            var bankAccount = new BankAccount();
            var controller = GetBankAccountController();
            bankAccountService.CheckIsAccountNameExisted(bankAccount).Returns(false);

            var actual = controller.CheckAccountNameExisted(bankAccount);
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.True((bool)jsonResult.Value);
        }

        [Fact]
        public void CheckAccountNumberExisted_AccountExisted_ReturnJsonWithMessage()
        {
            var bankAccount = new BankAccount();
            var controller = GetBankAccountController();
            bankAccountService.CheckIsAccountNumberExisted(bankAccount).Returns(true);

            var actual = controller.CheckAccountNumberExisted(bankAccount);
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(BankAccountError.AccountNumberExisted.GetDisplayName(), jsonResult.Value.ToString());
        }

        [Fact]
        public void CheckAccountNumberExisted_AccountNotExisted_ReturnJsonWithTrue()
        {
            var bankAccount = new BankAccount();
            var controller = GetBankAccountController();
            bankAccountService.CheckIsAccountNumberExisted(bankAccount).Returns(false);

            var actual = controller.CheckAccountNumberExisted(bankAccount);
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.True((bool)jsonResult.Value);
        }

        [Fact]
        public void EditBankAccount_ValidBankAccountId_ReturnJsonWithSuccess()
        {
            var controller = GetBankAccountController();
            var resources = GetResources();
            var bankAccountId = 1;
            var employeeId = 1;
            var companyId = 1;
            var bankAccount = new BankAccount { Id = bankAccountId, EmployeeId = employeeId, IsActive = true, BankType = 1 };
            var employeeBankAcount = new EmployeeBankAccounts
            {
                EmployeeId = employeeId,
                CompanyId = companyId,
                BankAccounts = new List<BankAccount> { bankAccount }
            };

            bankAccountService.GetBankAccountById(bankAccountId).Returns(bankAccount);
            bankAccountService.GetBankAccountsByEmployeeId(bankAccount.EmployeeId).Returns(employeeBankAcount);
            refResourceService.GetReferenceResourcesByNames(Arg.Any<string[]>()).Returns(resources);
            refResourceService.FilterReferenceResources(resources, ReferenceResource.BankType).Returns(resources.Where(x => x.RefName == ReferenceResource.BankType));
            refResourceService.FilterReferenceResources(resources, ReferenceResource.BankAccountType).Returns(resources.Where(x => x.RefName == ReferenceResource.BankAccountType));

            var actual = controller.EditBankAccount(employeeId);
            var viewResult = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<EditBankAccountViewModel>(viewResult.Model);
        }

        [Fact]
        public void EditBankAccount_ValidModel_ReturnJsonWithSuccess()
        {
            var controller = GetBankAccountController();
            var employeeId = 1;
            var model = new EditBankAccountViewModel
            {
                EmployeeId = employeeId,
                BankAccount = new BankAccount { EmployeeId = employeeId }
            };
            bankAccountService.UpdateBankAccount(model.BankAccount).Returns(new CommandResult { Code = 0 });
            controller.Url = Substitute.For<IUrlHelper>();
            controller.Url.Action().ReturnsForAnyArgs("/EmployeeBankAccounts/BankAccount");

            var actual = controller.EditBankAccount(model);

            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.True((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public void EditBankAccount_InValidModel_ReturnJsonWithError()
        {
            var controller = GetBankAccountController();
            var employeeId = 1;
            var model = new EditBankAccountViewModel
            {
                EmployeeId = employeeId,
                BankAccount = new BankAccount { EmployeeId = employeeId }
            };

            controller.ModelState.AddModelError(nameof(model.BankAccount.AccountName), "Error");
            var actual = controller.EditBankAccount(model);

            bankAccountService.DidNotReceive().UpdateBankAccount(model.BankAccount);
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public void GetEmployeeBankAccounts_ContraCoAccountType_ReturnContraCoAccounts()
        {
            var controller = GetBankAccountController();
            var refNames = new[] { ReferenceResource.ContraCoAccount };

            refResourceService.GetReferenceResourcesByNames(refNames).Returns(new List<ReferenceResourceModel>());

            var actual = controller.GetEmployeeBankAccounts((int)Core.Enumerations.BankType.ContraCoAccount, 1);

            baseService.DidNotReceive().GetData<BankAccount>(Arg.Any<SearchBankAccountCriteria>());
            refResourceService.Received().GetReferenceResourcesByNames(Arg.Any<string[]>());
            Assert.IsType<JsonResult>(actual);
        }

        [Fact]
        public void GetEmployeeBankAccounts_NotContraCoAccountType_ReturnEmployeeBankAccounts()
        {
            var controller = GetBankAccountController();

            baseService.GetData<BankAccount>(Arg.Any<SearchBankAccountCriteria>()).Returns(new List<BankAccount>());

            var actual = controller.GetEmployeeBankAccounts((int)Core.Enumerations.BankType.Ebanking, 1);

            baseService.Received().GetData<BankAccount>(Arg.Any<SearchBankAccountCriteria>());
            refResourceService.DidNotReceive().GetReferenceResourcesByNames(Arg.Any<string[]>());
            Assert.IsType<JsonResult>(actual);
        }

        private IEnumerable<ReferenceResourceModel> GetResources()
        {
            var resources = new List<ReferenceResourceModel>
                {
                    new ReferenceResourceModel
                    {
                        RefName = ReferenceResource.BankType,
                        ResourceId = (int) Core.Enumerations.BankType.Ebanking
                    },
                    new ReferenceResourceModel
                    {
                        RefName = ReferenceResource.BankType,
                        ResourceId = (int) Core.Enumerations.BankType.DirectBanking
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
            FakeSessionService();
            FakeBaseService();
            FakeBankAccountService();
            FakeRefResourceService();

            return new BankAccountController(baseService, bankAccountService, refResourceService, sessionService);
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