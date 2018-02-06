namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Extensions;
    using HRP.Core.Services;
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

        private IEnumerable<ReferenceResourceModel> GetResources()
        {
            var resources = new List<ReferenceResourceModel>
                {
                    new ReferenceResourceModel
                    {
                        RefName = ReferenceResource.BankType,
                        ResourceId = 1
                    },
                    new ReferenceResourceModel
                    {
                        RefName = ReferenceResource.BankType,
                        ResourceId = 2
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