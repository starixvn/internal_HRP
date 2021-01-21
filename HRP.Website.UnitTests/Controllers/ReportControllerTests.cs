namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Extensions;
    using Core.Model;
    using Core.Services;
    using Criteria.Employee;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Models;
    using Models.Report;
    using Newtonsoft.Json;
    using NSubstitute;
    using Services;
    using Website.Controllers;
    using Xunit;

    public class ReportControllerTests
    {
        private ISessionService sessionService;
        private IBaseService baseService;
        private IReportService reportService;
        private IHostingEnvironment hostingEnvironment;
        private IExportExcelService exportExcelService;
        private IRijndaelService rijndaelService;
        private ICommonService commonService;

        [Fact]
        public async Task GetEbankingPaymentTransaction_WhenCalled_ReturnEbanking()
        {
            // Arrange
            var reportController = GetReportController();

            var refResources = new List<ReferenceResourceModel>
            {
                new ReferenceResourceModel
                {
                    IsActive = true,
                    RefName = ReferenceResource.PaymentTransactionEbanking,
                    ResourceId = (int)BankTypes.Ebanking,
                    Value = BankTypes.DirectBanking.GetDisplayName()
                }
            };

            var expected = new List<MultiSelectViewModel>();
            expected.AddRange(refResources.Select(x => new MultiSelectViewModel
            {
                Value = $"{(int)BankTypes.Ebanking}-{x.ResourceId}",
                Text = x.Value
            }));

            commonService.GetReferenceResourcesByNamesAsync(Arg.Any<string[]>()).Returns(refResources);

            // Act
            var actual = await reportController.GetEbankingPaymentTransactions();

            //Assert
            var json = Assert.IsType<JsonResult>(actual);
            Assert.Equal(
                JsonConvert.SerializeObject(json.Value),
                JsonConvert.SerializeObject(expected));
        }

        [Fact]
        public async Task GetDirectBankingPaymentTransactions_WhenCalled_ReturnDirectBanking()
        {
            // Arrange
            var reportController = GetReportController();

            var refResources = new List<ReferenceResourceModel>
            {
                new ReferenceResourceModel
                {
                    IsActive = true,
                    RefName = ReferenceResource.PaymentTransactionDirectBanking,
                    ResourceId = (int)BankTypes.DirectBanking,
                    Value = BankTypes.DirectBanking.GetDisplayName()
                },
                new ReferenceResourceModel
                {
                    IsActive = true,
                    RefName = ReferenceResource.PaymentTransactionCash,
                    ResourceId = (int)BankTypes.Cash,
                    Value = BankTypes.Cash.GetDisplayName()
                }
            };

            var expected = new List<MultiSelectViewModel>
            {
                new MultiSelectViewModel
                {
                    Value = ((int)BankTypes.DirectBanking).ToString(),
                    Text = BankTypes.DirectBanking.GetDisplayName(),
                    ParentId = "0",
                    Items = refResources.Select(x => new MultiSelectViewModel
                    {
                        Value = $"{(int)BankTypes.DirectBanking}-{x.ResourceId}",
                        Text = x.Value,
                        ParentId = ((int)BankTypes.DirectBanking).ToString()
                    }).ToList()
                },
                new MultiSelectViewModel
                {
                    Value = ((int)BankTypes.Cash).ToString(),
                    Text = BankTypes.Cash.GetDisplayName(),
                    ParentId = "0",
                    Items = refResources.Select(x => new MultiSelectViewModel
                    {
                        Value = $"{(int)BankTypes.Cash}-{x.ResourceId}",
                        Text = x.Value,
                        ParentId = ((int)BankTypes.Cash).ToString()
                    }).ToList()
                }
            };

            commonService
                .FilterReferenceResources(Arg.Any<IEnumerable<ReferenceResourceModel>>(), Arg.Any<string>())
                .Returns(refResources);

            // Act
            var actual = await reportController.GetDirectBankingPaymentTransactions();

            // Assert
            var json = Assert.IsType<JsonResult>(actual);
            Assert.Equal(
                JsonConvert.SerializeObject(json.Value),
                JsonConvert.SerializeObject(expected));
        }

        [Fact]
        public async Task DirectBanking_WhenCalled_ReturnExpectedView()
        {
            // Arrange
            var rijndaelKeyValuePair = new RijndaelKeyValuePair
            {
                Key = "key",
                Value = "value"
            };
            var tuple = new Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair>(rijndaelKeyValuePair, rijndaelKeyValuePair);
            var reportController = GetReportController();
            reportController.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            rijndaelService.GenerateRijndaelKeyValuePair().Returns(tuple);

            // Act
            var actual = await reportController.DirectBanking();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<BankInReportViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task Ebanking_WhenCalled_ReturnExpectedView()
        {
            // Arrange
            var rijndaelKeyValuePair = new RijndaelKeyValuePair
            {
                Key = "key",
                Value = "value"
            };
            var tuple = new Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair>(rijndaelKeyValuePair, rijndaelKeyValuePair);
            var reportController = GetReportController();
            reportController.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            rijndaelService.GenerateRijndaelKeyValuePair().Returns(tuple);

            // Act
            var actual = await reportController.Ebanking();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<BankInReportViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task GetDirectBankingReport_InvalidParams_ReturnEmpty()
        {
            // Arrange
            var bankInReportViewModel = new BankInReportViewModel
            {
                CompanyId = 0,
                SelectedTime = DateTime.Now,
                TypeOfPayment = PaymentType.BasicSalary,
                RijndaelKey = new RijndaelKeyValuePair { Key = "key" },
                RijndaelIV = new RijndaelKeyValuePair { Key = "key" }
            };

            var reportController = GetReportController();
            reportController.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());

            // Act
            var actual = await reportController.GetDirectBankingReport(bankInReportViewModel);

            // Assert
            Assert.Equal(PaymentType.BasicSalary.GetDisplayName(), bankInReportViewModel.TypeOfPayment.GetDisplayName());
            Assert.IsType<RedirectToActionResult>(actual);
        }

        [Fact]
        public async Task GetDirectBankingReport_ValidParams_ReturnNull()
        {
            // Arrange
            var bankInReportViewModel = new BankInReportViewModel
            {
                CompanyId = 1,
                SelectedTime = DateTime.Now,
                EncryptedPassword = "123",
                ReusePassword = true,
                RijndaelKey = new RijndaelKeyValuePair { Key = "key" },
                RijndaelIV = new RijndaelKeyValuePair { Key = "key" }
            };

            var filePassword = "456";
            var companyName = "Dashword";
            var reportedMonth = DateTime.Today;

            var bankInData = new List<BankIn>
            {
                new BankIn { Amount = "1" },
                new BankIn { Amount = "1" },
            };

            var bankInReport = new BankInReport
            {
                EBankingBankIns = bankInData,
                CashBankIns = bankInData,
                CompanyName = companyName,
                ReportedMonth = reportedMonth
            };

            var reportController = GetReportController();
            reportController.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            reportService.GetDirectBankingBankInAsync(Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<PaymentType>(), Arg.Any<string>()).Returns(bankInReport);
            rijndaelService.Decrypt(bankInReportViewModel.EncryptedPassword, Arg.Any<string>(), Arg.Any<string>()).Returns(filePassword);
            reportController.ObjectValidator = Substitute.For<IObjectModelValidator>();
            reportController.ObjectValidator.When(x => x.Validate(Arg.Any<ActionContext>(), Arg.Any<ValidationStateDictionary>(), Arg.Any<string>(), Arg.Any<Object>()));

            // Act
            var actual = await reportController.GetDirectBankingReport(bankInReportViewModel);

            // Assert
            Assert.Equal(filePassword, bankInReportViewModel.FilePassword);
            Assert.IsType<FileContentResult>(actual);
        }

        [Fact]
        public async Task GetEbankingReport_InvalidParams_ReturnEmpty()
        {
            // Arrange
            var bankInReportViewModel = new BankInReportViewModel
            {
                CompanyId = 0,
                SelectedTime = DateTime.Now,
                TypeOfPayment = PaymentType.Allowances,
                RijndaelKey = new RijndaelKeyValuePair { Key = "key" },
                RijndaelIV = new RijndaelKeyValuePair { Key = "key" }
            };

            var reportController = GetReportController();
            reportController.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());

            // Act
            var actual = await reportController.GetEbankingReport(bankInReportViewModel);

            // Assert
            Assert.Equal(PaymentType.Allowances.GetDisplayName(), bankInReportViewModel.TypeOfPayment.GetDisplayName());
            Assert.IsType<RedirectToActionResult>(actual);
        }

        [Fact]
        public async Task GetEbankingReport_ValidParams_ReturnNull()
        {
            // Arrange
            var bankInReportViewModel = new BankInReportViewModel
            {
                CompanyId = 1,
                SelectedTime = DateTime.Now,
                EncryptedPassword = "456",
                ReusePassword = true,
                RijndaelKey = new RijndaelKeyValuePair { Key = "key" },
                RijndaelIV = new RijndaelKeyValuePair { Key = "key" }
            };

            var filePassword = "123";

            var companyName = "Dashword";
            var reportedMonth = DateTime.Today;

            var bankInData = new List<BankIn>
            {
                new BankIn { Amount = "1" },
                new BankIn { Amount = "1" },
            };

            var bankInReport = new BankInReport
            {
                EBankingBankIns = bankInData,
                CashBankIns = bankInData,
                CompanyName = companyName,
                ReportedMonth = reportedMonth
            };

            var reportController = GetReportController();
            reportController.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            reportService.GetEbankingBankInAsync(Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<string>()).Returns(bankInReport);
            rijndaelService.Decrypt(bankInReportViewModel.EncryptedPassword, Arg.Any<string>(), Arg.Any<string>()).Returns(filePassword);
            reportController.ObjectValidator = Substitute.For<IObjectModelValidator>();
            reportController.ObjectValidator.When(x => x.Validate(Arg.Any<ActionContext>(), Arg.Any<ValidationStateDictionary>(), Arg.Any<string>(), Arg.Any<Object>()));

            // Act
            var actual = await reportController.GetEbankingReport(bankInReportViewModel);

            // Assert
            Assert.Equal(filePassword, bankInReportViewModel.FilePassword);
            Assert.IsType<FileContentResult>(actual);
        }

        [Fact]
        public async Task DynamicReport_WhenCalled_ReturnExpectedViewResult()
        {
            // Arrange
            var companies = new List<Company>
            {
                new Company { CompanyId = 1, Name = "DAR" }
            };
            var companySelectList = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));
            var employeeStatuses = new List<MultiSelectViewModel>
            {
                new MultiSelectViewModel { Value = "1", Text = "Probation" },
                new MultiSelectViewModel { Value = "3", Text = "Resigned" }
            };

            var filePassword = "123";
            var encryptedPassword = "456";
            var rijndaelKeyValuePair = new RijndaelKeyValuePair
            {
                Key = "key",
                Value = "value"
            };
            var tuple = new Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair>(rijndaelKeyValuePair, rijndaelKeyValuePair);

            var reportController = GetReportController();
            reportController.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            commonService.GetSelectList(Arg.Any<IEnumerable<Company>>(), Arg.Any<string>(), Arg.Any<string>()).Returns(companySelectList);
            baseService.GetDataAsync<MultiSelectViewModel>(Arg.Any<GetEmployeeStatusByUserCriteria>()).Returns(employeeStatuses);
            rijndaelService.GenerateRijndaelKeyValuePair().Returns(tuple);
            reportService.GetUserFilePasswordAsync(sessionService.UserId).Returns(filePassword);
            rijndaelService.Encrypt(filePassword, Arg.Any<string>(), Arg.Any<string>()).Returns(encryptedPassword);

            // Act
            var actual = await reportController.DynamicReport();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsType<GenerateReportViewModel>(viewResult.Model);
            Assert.Equal("DynamicReport", viewResult.ViewName);
            Assert.Equal("1,2", viewModel.EmployeeStatusIds);
            Assert.Equal(companySelectList, viewModel.Companies);
            Assert.True(string.IsNullOrEmpty(viewModel.FilePassword));
            Assert.Equal(encryptedPassword, viewModel.EncryptedPassword);
        }

        [Fact]
        public async Task DynamicReport_ValidModel_ReturnFile()
        {
            // Arrange
            var viewModel = new GenerateReportViewModel
            {
                CompanyId = 1,
                EmployeeStatusIds = "1,2",
                SelectedIds = "1",
                ReusePassword = true,
                EncryptedPassword = "456",
                RijndaelKey = new RijndaelKeyValuePair { Key = "key" },
                RijndaelIV = new RijndaelKeyValuePair { Key = "key" }
            };

            var employeeDatas = new List<DynamicReportModel>
            {
                new DynamicReportModel
                {
                    IDNo = "DAR001"
                }
            };

            var reportController = GetReportController();
            reportController.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.DynamicReport, Policy.Read).Returns(true);
            reportService.GetEmployeeDataForDynamicReport(viewModel.CompanyId, viewModel.EmployeeStatusIds).Returns(employeeDatas);
            rijndaelService.Decrypt(viewModel.EncryptedPassword, Arg.Any<string>(), Arg.Any<string>()).Returns("123");
            exportExcelService.ExportDynamicReport(employeeDatas, viewModel.SelectedIds.Split(',').Select(int.Parse), "123").Returns(new byte[10]);
            reportController.ObjectValidator = Substitute.For<IObjectModelValidator>();
            reportController.ObjectValidator.When(x => x.Validate(Arg.Any<ActionContext>(), Arg.Any<ValidationStateDictionary>(), Arg.Any<string>(), Arg.Any<Object>()));

            // Act
            var actual = await reportController.DynamicReport(viewModel);

            // Assert
            Assert.IsType<FileContentResult>(actual);
        }

        [Fact]
        public async Task DynamicReport_InvalidModel_ReturnExpectedViewResult()
        {
            // Arrange
            var companies = new List<Company>
            {
                new Company { CompanyId = 1, Name = "DAR" }
            };
            var companySelectList = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));
            var viewModel = new GenerateReportViewModel
            {
                CompanyId = 1,
                EmployeeStatusIds = "1,2",
                SelectedIds = "1",
                RijndaelKey = new RijndaelKeyValuePair { Key = "key" },
                RijndaelIV = new RijndaelKeyValuePair { Key = "key" }
            };
            var rijndaelKeyValuePair = new RijndaelKeyValuePair
            {
                Key = "key",
                Value = "value"
            };
            var tuple = new Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair>(rijndaelKeyValuePair, rijndaelKeyValuePair);

            var reportController = GetReportController();
            reportController.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            reportController.ObjectValidator = Substitute.For<IObjectModelValidator>();
            reportController.ObjectValidator.When(x => x.Validate(Arg.Any<ActionContext>(), Arg.Any<ValidationStateDictionary>(),
                 Arg.Any<string>(), Arg.Any<Object>())).Do(x => reportController.ModelState.AddModelError(nameof(viewModel.FilePassword), "Password is required"));
            sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.DynamicReport, Policy.Read).Returns(true);
            commonService.GetSelectList(Arg.Any<IEnumerable<Company>>(), Arg.Any<string>(), Arg.Any<string>()).Returns(companySelectList);
            rijndaelService.GenerateRijndaelKeyValuePair().Returns(tuple);

            // Act
            var actual = await reportController.DynamicReport(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModelResult = Assert.IsType<GenerateReportViewModel>(viewResult.Model);
            Assert.Equal(viewModel.CompanyId, viewModelResult.CompanyId);
            Assert.Equal(viewModel.EmployeeStatusIds, viewModelResult.EmployeeStatusIds);
            Assert.Equal(viewModel.SelectedIds, viewModelResult.SelectedIds);
            Assert.Equal(companySelectList, viewModelResult.Companies);
        }

        [Fact]
        public async Task DynamicReport_DontHavePermissionOnCompany_RedirectToAccessDenied()
        {
            // Arrange
            var viewModel = new GenerateReportViewModel
            {
                CompanyId = 1,
                EmployeeStatusIds = "1,2",
                SelectedIds = "1"
            };

            var reportController = GetReportController();
            sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.DynamicReport, Policy.Read).Returns(false);

            // Act
            var actual = await reportController.DynamicReport(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("AccessDenied", redirectResult.ActionName);
            Assert.Equal("Error", redirectResult.ControllerName);
        }

        #region Helpers

        private static ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId = 1;

            return service;
        }

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private static IReportService FakeReportService()
        {
            return Substitute.For<IReportService>();
        }

        private static IHostingEnvironment FakeHostingEnviroment()
        {
            return Substitute.For<IHostingEnvironment>();
        }

        private static IExportExcelService FakeExportExcelService()
        {
            return Substitute.For<IExportExcelService>();
        }

        private static ICommonService FakeCommonService()
        {
            return Substitute.For<ICommonService>();
        }

        private static IRijndaelService FakeRijndaelService()
        {
            var service = Substitute.For<IRijndaelService>();
            var tuple = new Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair>(new RijndaelKeyValuePair(), new RijndaelKeyValuePair());
            service.GenerateRijndaelKeyValuePair().Returns(tuple);

            return service;
        }

        private ReportController GetReportController()
        {
            sessionService = FakeSessionService();
            baseService = FakeBaseService();
            reportService = FakeReportService();
            exportExcelService = FakeExportExcelService();
            hostingEnvironment = FakeHostingEnviroment();
            rijndaelService = FakeRijndaelService();
            commonService = FakeCommonService();

            return new ReportController(
                baseService,
                sessionService,
                hostingEnvironment,
                exportExcelService,
                reportService,
                rijndaelService,
                commonService);
        }

        #endregion Helpers
    }
}