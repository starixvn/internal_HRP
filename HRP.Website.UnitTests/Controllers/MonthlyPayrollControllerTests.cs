namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Model;
    using Criteria;
    using HRP.Core.Criteria;
    using HRP.Models.BankAccount;
    using HRP.Models.MonthlyPayroll.EmployeeMonthlyPayroll;
    using HRP.Models.ServiceModels;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Routing;
    using Models;
    using Models.Currency;
    using Models.MonthlyPayroll;
    using NSubstitute;
    using Xunit;

    public partial class PaymentProcessControllerTests
    {
        #region EmployeeMonthlyPayroll

        [Fact]
        public async Task EmployeeMonthlyPayroll_HasNotPermission_ReturnRedirectToAccessDenied()
        {
            // Arrange
            var month = new DateTime(2021, 3, 1);
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(1, "PaymentProcess", Policy.Read).Returns(false);

            // Act
            var actual = await controller.EmployeeMonthlyPayroll(1, month);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("AccessDenied", viewResult.ActionName);
        }

        [Fact]
        public async Task EmployeeMonthlyPayroll_HasPermission_ReturnAsExpectedViewResult()
        {
            // Arrange
            var companyId = 1;
            var employeeId = 31;
            var month = DateTime.Now;
            var tuple = (new RijndaelKeyValuePair(), new RijndaelKeyValuePair());
            var cipherText = "1234abc";
            var filterInfo = new MonthlyPayrollFilterInfoModel();
            var employeePayrolls = new List<EmployeeMonthlyPayrollModel>
            {
                new EmployeeMonthlyPayrollModel { }
            };
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(companyId, Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);
            monthlyPayrollService.GetFilterInfoAsync(companyId, month).Returns(filterInfo);
            monthlyPayrollService.GetMonthlyPayrollAsync(Arg.Any<EmployeeMonthlyPayrollViewModel>()).Returns(employeePayrolls);
            rijndaelService.GenerateRijndaelKeyValuePair().Returns(tuple);
            rijndaelService.Encrypt(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(cipherText);

            // Act
            var actual = await controller.EmployeeMonthlyPayroll(companyId, month, employeeId, null, null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModelResult = Assert.IsAssignableFrom<EmployeeMonthlyPayrollViewModel>(viewResult.Model);
            Assert.Equal("MonthlyPayroll", viewResult.ViewName);
            Assert.Equal(filterInfo, viewModelResult.FilterInfo);
            Assert.Equal(month, viewModelResult.Month);
            Assert.Equal(employeeId, viewModelResult.EmployeeId);
            Assert.Null(viewModelResult.Employee);
            Assert.Null(viewModelResult.IDNo);
        }

        #endregion EmployeeMonthlyPayroll

        #region EditBasicSalary

        [Fact]
        public async Task EditBasicSalary_HasNotPermission_ReturnRedirectToAccessDenied()
        {
            // Arrange
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(false);

            // Act
            var actual = await controller.EditBasicSalary(1, 1, DateTime.Today, new CachingFilterViewModel());

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("AccessDenied", viewResult.ActionName);
        }

        [Fact]
        public async Task EditBasicSalary_MonthlyPayrollIsNotExisted_ReturnRedirectToPageNotFound()
        {
            // Arrange
            MonthlyBasicSalaryModel monthlyBasicSalary = null;
            var monthlyPayroll = (
                    monthlyBasicSalary,
                    new MonthlySalaryModel(),
                    new List<MonthlySalaryAdditionModel>(),
                    new List<MonthlySalaryDeductionModel>());
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);
            monthlySalaryService.GetMonthlySalaryAsync(Arg.Any<int>()).Returns(monthlyPayroll);

            // Act
            var actual = await controller.EditBasicSalary(1, 1, DateTime.Today, new CachingFilterViewModel());

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("PageNotFound", viewResult.ActionName);
        }

        [Fact]
        public async Task EditBasicSalary_MonthlyPayrollIsExisted_ReturnAsExpectedViewResult()
        {
            // Arrange
            var companyId = 1;
            var payrollId = 1;
            var month = DateTime.Now;
            var monthlyPayroll = (new MonthlyBasicSalaryModel { WorkingDays = 10 },
                    new MonthlySalaryModel { CurrencyCode = "USD" },
                    new List<MonthlySalaryAdditionModel> { new MonthlySalaryAdditionModel() },
                    new List<MonthlySalaryDeductionModel> { new MonthlySalaryDeductionModel() });
            var filterInfo = new MonthlyPayrollFilterInfoModel();
            var currencyExchangeRates = new List<CurrencyExchangeRateModel>();
            var cachingFilter = new CachingFilterViewModel();
            var rijndaelKeyValuePair = new RijndaelKeyValuePair
            {
                Key = "key",
                Value = "value"
            };
            var tuple = (rijndaelKeyValuePair, rijndaelKeyValuePair);
            var cipherText = "1234abc";
            var bankAccounts = new List<BankAccount>
            {
                new BankAccount { BankAccountId = 1, PaymentTransaction = "Eurobank" }
            };
            var bankAccountSelectList = new SelectList(bankAccounts, nameof(BankAccount.BankAccountId), nameof(BankAccount.PaymentTransaction));
            var cashAccounts = new List<ReferenceResourceModel>
            {
                new ReferenceResourceModel { ResourceId = 1, Value = "Cash - DAR" }
            };
            var cashAccountSelectList = new SelectList(cashAccounts, nameof(ReferenceResourceModel.ResourceId), nameof(ReferenceResourceModel.Value));

            var controller = GetPaymentProcessController();
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            sessionService.HasPermissionOnCompany(companyId, Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);
            monthlySalaryService.GetMonthlySalaryAsync(Arg.Any<int>()).Returns(monthlyPayroll);
            monthlyPayrollService.GetFilterInfoAsync(companyId, month, payrollId).Returns(filterInfo);
            baseService.GetDataAsync<CurrencyExchangeRateModel>(Arg.Any<GetExchangeRateCriteria>()).Returns(currencyExchangeRates);
            rijndaelService.GenerateRijndaelKeyValuePair().Returns(tuple);
            rijndaelService.Encrypt(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(cipherText);
            commonService.GetSelectList(Arg.Any<IEnumerable<BankAccount>>(), nameof(BankAccount.BankAccountId), nameof(BankAccount.PaymentTransaction))
                .Returns(bankAccountSelectList);
            commonService.GetSelectList(Arg.Any<IEnumerable<ReferenceResourceModel>>(), nameof(ReferenceResourceModel.ResourceId), nameof(ReferenceResourceModel.Value))
                .Returns(cashAccountSelectList);

            // Act
            var actual = await controller.EditBasicSalary(payrollId, companyId, month, cachingFilter);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModelResult = Assert.IsAssignableFrom<EditMonthlySalaryViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
            Assert.Equal(month, viewModelResult.Month);
            Assert.Equal(cachingFilter, viewModelResult.CachingFilter);

            Assert.Equal(monthlyPayroll.Item1.CurrencyId, viewModelResult.MonthlyBasicSalary.CurrencyId);
            Assert.Equal(monthlyPayroll.Item1.CurrencyCode, viewModelResult.MonthlyBasicSalary.CurrencyCode);
            Assert.Equal(cipherText, viewModelResult.MonthlyBasicSalary.Salary.EncryptedValue);
            Assert.Equal(monthlyPayroll.Item1.EbankingCurrencyId, viewModelResult.MonthlyBasicSalary.EbankingCurrencyId);
            Assert.Equal(monthlyPayroll.Item1.EbankingCurrencyCode, viewModelResult.MonthlyBasicSalary.EbankingCurrencyCode);
            Assert.Equal(cipherText, viewModelResult.MonthlyBasicSalary.EbankingAmount.EncryptedValue);
            Assert.Equal(monthlyPayroll.Item1.DirectBanking1CurrencyId, viewModelResult.MonthlyBasicSalary.DirectBanking1CurrencyId);
            Assert.Equal(monthlyPayroll.Item1.DirectBanking1CurrencyCode, viewModelResult.MonthlyBasicSalary.DirectBanking1CurrencyCode);
            Assert.Equal(cipherText, viewModelResult.MonthlyBasicSalary.DirectBanking1Amount.EncryptedValue);
            Assert.Equal(monthlyPayroll.Item1.DirectBanking2CurrencyId, viewModelResult.MonthlyBasicSalary.DirectBanking2CurrencyId);
            Assert.Equal(monthlyPayroll.Item1.DirectBanking2CurrencyCode, viewModelResult.MonthlyBasicSalary.DirectBanking2CurrencyCode);
            Assert.Equal(cipherText, viewModelResult.MonthlyBasicSalary.DirectBanking2Amount.EncryptedValue);
            Assert.Equal(monthlyPayroll.Item1.CashCurrencyId, viewModelResult.MonthlyBasicSalary.CashCurrencyId);
            Assert.Equal(monthlyPayroll.Item1.CashCurrencyCode, viewModelResult.MonthlyBasicSalary.CashCurrencyCode);
            Assert.Equal(cipherText, viewModelResult.MonthlyBasicSalary.CashAmount.EncryptedValue);
            Assert.Equal(cipherText, viewModelResult.MonthlyBasicSalary.AfterAdjustmentAmount.EncryptedValue);
            Assert.Equal(cipherText, viewModelResult.MonthlyBasicSalary.AfterAdjustmentReason.EncryptedValue);
            Assert.Equal(cipherText, viewModelResult.MonthlyBasicSalary.SalaryPerDay.EncryptedValue);
            Assert.Equal(monthlyPayroll.Item1.WorkingDays, viewModelResult.MonthlyBasicSalary.WorkingDays);

            Assert.Equal(monthlyPayroll.Item2.CurrencyId, viewModelResult.MonthlySalary.Salary.CurrencyId);
            Assert.Equal(cipherText, viewModelResult.MonthlySalary.Salary.EncryptedValue);
            Assert.Equal(monthlyPayroll.Item2.EbankingCurrencyId, viewModelResult.MonthlySalary.Ebanking.CurrencyId);
            Assert.Equal(cipherText, viewModelResult.MonthlySalary.Ebanking.EncryptedValue);
            Assert.Equal(monthlyPayroll.Item2.EbankingAccountId, viewModelResult.MonthlySalary.Ebanking.AccountId);
            Assert.Equal(monthlyPayroll.Item2.EbankingAccountName, viewModelResult.MonthlySalary.Ebanking.AccountName);
            Assert.Equal(monthlyPayroll.Item2.DirectBanking1CurrencyId, viewModelResult.MonthlySalary.DirectBanking1.CurrencyId);
            Assert.Equal(cipherText, viewModelResult.MonthlySalary.DirectBanking1.EncryptedValue);
            Assert.Equal(monthlyPayroll.Item2.DirectBanking1AccountId, viewModelResult.MonthlySalary.DirectBanking1.AccountId);
            Assert.Equal(monthlyPayroll.Item2.DirectBanking1AccountName, viewModelResult.MonthlySalary.DirectBanking1.AccountName);
            Assert.Equal(monthlyPayroll.Item2.DirectBanking2CurrencyId, viewModelResult.MonthlySalary.DirectBanking2.CurrencyId);
            Assert.Equal(cipherText, viewModelResult.MonthlySalary.DirectBanking2.EncryptedValue);
            Assert.Equal(monthlyPayroll.Item2.DirectBanking2AccountId, viewModelResult.MonthlySalary.DirectBanking2.AccountId);
            Assert.Equal(monthlyPayroll.Item2.DirectBanking2AccountName, viewModelResult.MonthlySalary.DirectBanking2.AccountName);
            Assert.Equal(monthlyPayroll.Item2.DirectBanking1AccountName, viewModelResult.MonthlySalary.DirectBanking1.AccountName);
            Assert.Equal(monthlyPayroll.Item2.CashCurrencyId, viewModelResult.MonthlySalary.Cash.CurrencyId);
            Assert.Equal(cipherText, viewModelResult.MonthlySalary.Cash.EncryptedValue);
            Assert.Equal(monthlyPayroll.Item2.CashAccountId, viewModelResult.MonthlySalary.Cash.AccountId);
            Assert.Equal(monthlyPayroll.Item2.AutoCalculateOn, viewModelResult.MonthlySalary.AutoCalculateOn);
            Assert.Equal(cipherText, viewModelResult.MonthlySalary.AdditionAmount.EncryptedValue);
            Assert.Equal(cipherText, viewModelResult.MonthlySalary.AdditionAmountDisplay.EncryptedValue);
            Assert.Equal(cipherText, viewModelResult.MonthlySalary.DeductionAmount.EncryptedValue);
            Assert.Equal(cipherText, viewModelResult.MonthlySalary.DeductionAmountDisplay.EncryptedValue);

            Assert.Collection(
                viewModelResult.Additions,
                kvp =>
                {
                    Assert.Equal(cipherText, kvp.EncryptedValue);
                    Assert.Equal(cipherText, kvp.NetSalary.EncryptedValue);
                    Assert.Equal(cipherText, kvp.NetSalaryDisplay.EncryptedValue);
                });

            Assert.Collection(
                viewModelResult.Deductions,
                kvp =>
                {
                    Assert.Equal(cipherText, kvp.EncryptedValue);
                    Assert.Equal(cipherText, kvp.NetSalary.EncryptedValue);
                    Assert.Equal(cipherText, kvp.NetSalaryDisplay.EncryptedValue);
                });

            Assert.Equal(currencyExchangeRates, viewModelResult.CurrencyExchangeRates);
            Assert.Equal(filterInfo, viewModelResult.FilterInfo);
            Assert.Equal(bankAccountSelectList, viewModelResult.EBankingAccounts);
            Assert.Equal(bankAccountSelectList, viewModelResult.DirectBankingAccounts);
            Assert.Equal(cashAccountSelectList, viewModelResult.CashAccounts);
        }

        [Fact]
        public async Task EditBasicSalaryPost_HasNotPermission_ReturnRedirectToAccessDenied()
        {
            // Arrange
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(false);

            // Act
            var actual = await controller.EditBasicSalary(new EditMonthlySalaryViewModel());

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("AccessDenied", viewResult.ActionName);
        }

        [Fact]
        public async Task EditBasicSalaryPost_ModelStateIsInvalid_ReturnAsExpectedErrors()
        {
            // Arrange
            var viewModel = new EditMonthlySalaryViewModel
            {
                MonthlySalary = new MonthlySalaryViewModel(),
                Additions = new List<MonthlySalaryAdditionViewModel> { new MonthlySalaryAdditionViewModel() },
                Deductions = new List<MonthlySalaryDeductionViewModel> { new MonthlySalaryDeductionViewModel() },
                RijndaelKey = new RijndaelKeyValuePair { Key = "key" },
                RijndaelIV = new RijndaelKeyValuePair { Key = "key" }
            };
            var controller = GetPaymentProcessController();
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());

            var objectValidator = Substitute.For<IObjectModelValidator>();
            objectValidator.When(x => x.Validate(Arg.Any<ActionContext>(), Arg.Any<ValidationStateDictionary>(), Arg.Any<string>(), Arg.Any<Object>()))
                 .Do(x => controller.ModelState.AddModelError("UnitAmount", "required"));
            controller.ObjectValidator = objectValidator;
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);

            // Act
            var actual = await controller.EditBasicSalary(viewModel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var errors = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value.GetType().GetProperty("errors").GetValue(jsonResult.Value));
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
            Assert.Collection(
                errors,
                kvp =>
                {
                    Assert.Equal("UnitAmount", kvp.Key);
                    Assert.Equal("required", kvp.Value);
                }
            );
        }

        [Fact]
        public async Task EditBasicSalaryPost_ModelStateIsValid_ReturnAsExpected()
        {
            // Arrange
            var viewModel = new EditMonthlySalaryViewModel
            {
                Month = DateTime.Now,
                MonthlySalary = new MonthlySalaryViewModel(),
                Additions = new List<MonthlySalaryAdditionViewModel> { new MonthlySalaryAdditionViewModel() },
                Deductions = new List<MonthlySalaryDeductionViewModel> { new MonthlySalaryDeductionViewModel() },
                RijndaelKey = new RijndaelKeyValuePair { Key = "key" },
                RijndaelIV = new RijndaelKeyValuePair { Key = "key" }
            };
            var commandResult = new CommandResult();
            var controller = GetPaymentProcessController();
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            var objectValidator = Substitute.For<IObjectModelValidator>();
            objectValidator.Validate(Arg.Any<ActionContext>(),
                Arg.Any<ValidationStateDictionary>(),
                Arg.Any<string>(),
                Arg.Any<Object>());
            controller.ObjectValidator = objectValidator;
            controller.Url = Substitute.For<IUrlHelper>();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);
            monthlySalaryService.EditMonthlySalaryAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                viewModel.Month,
                Arg.Any<MonthlySalaryModel>(), Arg.Any<IEnumerable<MonthlySalaryAdditionModel>>(),
                Arg.Any<IEnumerable<MonthlySalaryDeductionModel>>()).Returns(commandResult);

            // Act
            var actual = await controller.EditBasicSalary(viewModel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(commandResult.IsSuccessful, (bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
            Assert.Equal(commandResult.Message, jsonResult.Value.GetType().GetProperty("message").GetValue(jsonResult.Value));
        }

        #endregion EditBasicSalary

        [Fact]
        public async Task GetEmployeeSuggestionByName_WhenCalled_ReturnAsExpectedEmployees()
        {
            // Arrange
            var expected = new List<EmployeeSuggestionViewModel>
            {
                new EmployeeSuggestionViewModel { EmployeeId = 1, IDNo = "DAR111", FullName = "Tim", Nickname = "tim.trinh" }
            };
            var controller = GetPaymentProcessController();
            monthlyPayrollService.GetEmployeeSuggestionByNameAsync(Arg.Any<int>(), Arg.Any<string>()).Returns(expected);

            // Act
            var actual = await controller.GetEmployeeSuggestionByName(1, "tim");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task GetEmployeeSuggestionByIDNo_WhenCalled_ReturnAsExpectedEmployees()
        {
            // Arrange
            var expected = new List<EmployeeSuggestionViewModel>
            {
                new EmployeeSuggestionViewModel { EmployeeId = 1, IDNo = "DAR111", FullName = "Tim", Nickname = "tim.trinh" }
            };
            var controller = GetPaymentProcessController();
            monthlyPayrollService.GetEmployeeSuggestionByIDNoAsync(Arg.Any<int>(), Arg.Any<string>()).Returns(expected);

            // Act
            var actual = await controller.GetEmployeeSuggestionByIDNo(1, "DAR111");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task GetHierarchyCompany_WhenCalled_ReturnAsExpectedHierarchy()
        {
            // Arrange
            var expected = new List<MultiSelectViewModel>();
            var controller = GetPaymentProcessController();
            monthlyPayrollService.GetHierarchyCompanyAsync(1, "1,2").Returns(expected);

            // Act
            var actual = await controller.GetHierarchyCompany(1, "1,2");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        #region EditAllowances

        [Fact]
        public async Task EditAllowances_NotHavePermission_RedirectToAccessDenied()
        {
            var companyId = 1;
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Read).Returns(false);

            var actual = await controller.EditAllowances(1, companyId, DateTime.Today, new CachingFilterViewModel());

            var result = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", result.ControllerName);
            Assert.Equal("AccessDenied", result.ActionName);
        }

        [Fact]
        public async Task EditAllowances_HavePermission_ReturnView()
        {
            var rijndaelKeyValuePair = new RijndaelKeyValuePair
            {
                Key = "key",
                Value = "value"
            };
            var tuple = (rijndaelKeyValuePair, rijndaelKeyValuePair);
            var month = DateTime.Today;
            var payrollId = 1;
            var companyId = 1;
            var controller = GetPaymentProcessController();
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            rijndaelService.Encrypt(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(string.Empty);
            rijndaelService.GenerateRijndaelKeyValuePair().Returns(tuple);
            sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Read).Returns(true);
            monthlyPayrollService.GetFilterInfoAsync(companyId, month, payrollId).Returns(new MonthlyPayrollFilterInfoModel());
            monthlyAllowancesService.GetMonthlyAllowancesAsync(payrollId, month)
                .Returns(new MonthlyAllowancesModel
                {
                    Allowance = new MonthlyAllowanceModel()
                });
            baseService.GetDataAsync<CurrencyCodeModel>(Arg.Any<GetActiveCurrencyCodesCriteria>()).Returns(new List<CurrencyCodeModel>());
            baseService.GetDataAsync<CurrencyExchangeRateModel>(Arg.Any<GetExchangeRateCriteria>()).Returns(new List<CurrencyExchangeRateModel>());
            baseService.GetDataAsync<ReferenceResourceModel>(Arg.Any<GetRefResourceByNamesCriteria>()).Returns(new List<ReferenceResourceModel>());

            var actual = await controller.EditAllowances(payrollId, companyId, month, new CachingFilterViewModel());

            var result = Assert.IsType<ViewResult>(actual);
            Assert.IsType<EditMonthlyAllowancesViewModel>(result.Model);
        }

        [Fact]
        public async Task EditAllowancesPost_HasNotPermission_ReturnRedirectToAccessDenied()
        {
            // Arrange
            var viewModel = new EditMonthlyAllowancesViewModel();
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(false);

            // Act
            var actual = await controller.EditAllowances(viewModel, EditMonthlyAllowanceAction.Save);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("AccessDenied", viewResult.ActionName);
        }

        [Fact]
        public async Task EditAllowancesPost_ModelStateIsInvalid_ReturnAsExpectedErrors()
        {
            // Arrange
            var viewModel = new EditMonthlyAllowancesViewModel
            {
                RijndaelKey = new RijndaelKeyValuePair { Key = "key" },
                RijndaelIV = new RijndaelKeyValuePair { Key = "key" }
            };
            var controller = GetPaymentProcessController();
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);
            controller.ObjectValidator = Substitute.For<IObjectModelValidator>();
            controller.ObjectValidator.When(x => x.Validate(Arg.Any<ActionContext>(), Arg.Any<ValidationStateDictionary>(),
                 Arg.Any<string>(), Arg.Any<Object>())).Do(x => controller.ModelState.AddModelError("UnitAmount", "required"));

            // Act
            var actual = await controller.EditAllowances(viewModel, EditMonthlyAllowanceAction.Save);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var errors = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value.GetType().GetProperty("errors").GetValue(jsonResult.Value));
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
            Assert.Collection(
                errors,
                kvp =>
                {
                    Assert.Equal("UnitAmount", kvp.Key);
                    Assert.Equal("required", kvp.Value);
                }
            );
        }

        [Fact]
        public async Task EditAllowancesPost_ModelStateIsValid_ReturnAsExpected()
        {
            // Arrange
            var viewModel = new EditMonthlyAllowancesViewModel
            {
                Allowance = new MonthlyAllowanceViewModel
                {
                    DirectBanking1 = new PaymentTransactionModel(),
                    DirectBanking2 = new PaymentTransactionModel(),
                    Cash = new PaymentTransactionModel(),
                    Total = new CurrencyAmountModel(),
                    TotalNet = new SecurityData(),
                    DST = new CurrencyAmountModel(),
                    Addition = new SecurityData(),
                    Deduction = new SecurityData()
                },
                RijndaelKey = new RijndaelKeyValuePair { Key = "key" },
                RijndaelIV = new RijndaelKeyValuePair { Key = "key" }
            };
            var commandResult = new CommandResult();
            var controller = GetPaymentProcessController();
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            controller.ObjectValidator = Substitute.For<IObjectModelValidator>();
            controller.ObjectValidator.When(x => x.Validate(Arg.Any<ActionContext>(), Arg.Any<ValidationStateDictionary>(),
                 Arg.Any<string>(), Arg.Any<Object>()));
            controller.Url = Substitute.For<IUrlHelper>();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);
            monthlyAllowancesService.EditMonthlyAllowancesAsync(Arg.Any<MonthlyAllowancesModel>(), sessionService.UserId).Returns(commandResult);

            // Act
            var actual = await controller.EditAllowances(viewModel, EditMonthlyAllowanceAction.Save);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(commandResult.IsSuccessful, (bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
            Assert.Equal(commandResult.Message, jsonResult.Value.GetType().GetProperty("message").GetValue(jsonResult.Value));
        }

        #endregion EditAllowances

        #region Preset

        [Fact]
        public async Task Preset_NotHavePermission_RedirectToAccessDenied()
        {
            // Arrange
            var companyId = 1;
            var employeeId = 1;
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Host).Returns(false);

            // Act
            var actual = await controller.Preset(companyId, employeeId, DateTime.Today);

            // Assert
            var result = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", result.ControllerName);
            Assert.Equal("AccessDenied", result.ActionName);
        }

        [Fact]
        public async Task Preset_HavePermission_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var companyId = 1;
            var employeeId = 1;
            var commandResult = new CommandResult { Code = 0 };
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Host).Returns(true);
            paymentProcessService.GenerateMonthlyPayrollAsync(companyId, DateTime.Today, employeeId).Returns(commandResult);

            // Act
            var actual = await controller.Preset(companyId, employeeId, DateTime.Today);

            // Assert
            var result = Assert.IsType<JsonResult>(actual);
            Assert.Equal(commandResult, result.Value);
        }

        #endregion Preset

        #region PresetAllMonthlyPayroll

        [Fact]
        public async Task PresetAllMonthlyPayroll_NotHavePermission_RedirectToAccessDenied()
        {
            // Arrange
            var companyId = 1;
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Host).Returns(false);

            // Act
            var actual = await controller.PresetAllMonthlyPayroll(companyId, DateTime.Today);

            // Assert
            var result = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", result.ControllerName);
            Assert.Equal("AccessDenied", result.ActionName);
        }

        [Fact]
        public async Task PresetAllMonthlyPayroll_HavePermission_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var companyId = 1;
            var commandResult = new CommandResult { Code = 0 };
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Host).Returns(true);
            paymentProcessService.GenerateMonthlyPayrollAsync(companyId, DateTime.Today, null).Returns(commandResult);

            // Act
            var actual = await controller.PresetAllMonthlyPayroll(companyId, DateTime.Today);

            // Assert
            var result = Assert.IsType<JsonResult>(actual);
            Assert.Equal(commandResult, result.Value);
        }

        #endregion PresetAllMonthlyPayroll
    }
}