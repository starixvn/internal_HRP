namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Extensions;
    using Core.Model;
    using Core.Services;
    using Criteria.Employee;
    using Criteria;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models;
    using Models.Employee;
    using Models.Report;
    using Newtonsoft.Json;
    using NSubstitute;
    using Resources;
    using Services;
    using Services.Interfaces;
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
            var reportController = GetReportController();

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
            var reportController = GetReportController();

            // Act
            var actual = await reportController.Ebanking();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<BankInReportViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task GetDirectBankingReport_InValidParams_ReturnEmpty()
        {
            // Arrange
            var bankInReportViewModel = new BankInReportViewModel
            {
                CompanyId = 0,
                SelectedTime = DateTime.Now,
                TypeOfPayment = PaymentType.BasicSalary
            };

            var reportController = GetReportController();

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
                SelectedTime = DateTime.Now
            };

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
            reportService.GetDirectBankingBankInAsync(Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<PaymentType>(), Arg.Any<string>()).Returns(bankInReport);

            // Act
            var actual = await reportController.GetDirectBankingReport(bankInReportViewModel);

            // Assert
            Assert.IsType<FileContentResult>(actual);
        }

        [Fact]
        public async Task GetEbankingReport_InValidParams_ReturnEmpty()
        {
            // Arrange
            var bankInReportViewModel = new BankInReportViewModel
            {
                CompanyId = 0,
                SelectedTime = DateTime.Now,
                TypeOfPayment = PaymentType.Allowances
            };

            var reportController = GetReportController();

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
                SelectedTime = DateTime.Now
            };

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
            reportService.GetEbankingBankInAsync(Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<string>()).Returns(bankInReport);

            // Act
            var actual = await reportController.GetEbankingReport(bankInReportViewModel);

            // Assert
            Assert.IsType<FileContentResult>(actual);
        }

        #region MonthlyPayroll

        [Fact]
        public async Task MonthlyPayroll_WhenCalled_ReturnAsExpectedViewResult()
        {
            // Arrange
            var companies = new List<Company> { new Company { CompanyId = 1, Name = "DAR" } };
            var companySelectList = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));
            var reportController = GetReportController();
            commonService.GetSelectList(Arg.Any<IEnumerable<Company>>(), nameof(Company.CompanyId), nameof(Company.Code))
                        .Returns(companySelectList);

            // Act
            var actual = await reportController.MonthlyPayroll();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewResultModel = Assert.IsType<MonthlyPayrollReportViewModel>(viewResult.Model);
            Assert.Equal("MonthlyPayroll", viewResult.ViewName);
            Assert.Equal(companySelectList, viewResultModel.Companies);
        }

        [Fact]
        public async Task MonthlyPayrollPost_UserNotHasPermissionOnThisCompany_RedirectToAccessDenied()
        {
            // Arrange
            var viewModel = new MonthlyPayrollReportViewModel
            {
                SelectedTime = DateTime.Today,
                CompanyId = 22
            };
            var reportController = GetReportController();
            sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.MonthlyPayrollReport, Policy.Read).Returns(false);

            // Act
            var actual = await reportController.MonthlyPayroll(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("AccessDenied", redirectResult.ActionName);
            Assert.Equal("Error", redirectResult.ControllerName);
        }

        [Fact]
        public async Task MonthlyPayrollPost_ModelStateIsInvalid_ReturnMonthlyPayrollViewResult()
        {
            // Arrange
            var viewModel = new MonthlyPayrollReportViewModel
            {
                SelectedTime = DateTime.Today,
                CompanyId = 22,
                CommonError = "ahihi"
            };

            var companies = new List<Company> { new Company { CompanyId = 1, Name = "DAR" } };
            var companySelectList = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));
            var reportController = GetReportController();
            reportController.ModelState.AddModelError("CompanyId", "Required");
            commonService.GetSelectList(Arg.Any<IEnumerable<Company>>(), nameof(Company.CompanyId), nameof(Company.Code))
                        .Returns(companySelectList);
            sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.MonthlyPayrollReport, Policy.Read).Returns(true);

            // Act
            var actual = await reportController.MonthlyPayroll(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewResultModel = Assert.IsType<MonthlyPayrollReportViewModel>(viewResult.Model);
            Assert.Equal("MonthlyPayroll", viewResult.ViewName);
            Assert.Equal(viewModel.SelectedTime, viewResultModel.SelectedTime);
            Assert.Equal(viewModel.CompanyId, viewResultModel.CompanyId);
            Assert.Equal(companySelectList, viewResultModel.Companies);
            Assert.Equal("Required", viewResult.ViewData.ModelState["CompanyId"].Errors.FirstOrDefault().ErrorMessage);
        }

        [Fact]
        public async Task MonthlyPayrollPost_ReportNotHasAnyEmployees_ReturnMonthlyPayrollViewResult()
        {
            // Arrange
            var report = new MonthlyPayrollReport
            {
                Employees = new List<EmployeeBasicInfo>(),
                Month = DateTime.Today
            };
            var viewModel = new MonthlyPayrollReportViewModel
            {
                SelectedTime = DateTime.Today,
                CompanyId = 22
            };
            var companies = new List<Company> { new Company { CompanyId = 1, Name = "DAR" } };
            var companySelectList = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));

            var reportController = GetReportController();
            sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.MonthlyPayrollReport, Policy.Read).Returns(true);
            reportService.GetDataForMonthlyPayrollReportAsync(viewModel.SelectedTime, viewModel.CompanyId).Returns(report);
            commonService.GetSelectList(Arg.Any<IEnumerable<Company>>(), nameof(Company.CompanyId), nameof(Company.Code))
                        .Returns(companySelectList);
            sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.MonthlyPayrollReport, Policy.Read).Returns(true);

            // Act
            var actual = await reportController.MonthlyPayroll(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewResultModel = Assert.IsType<MonthlyPayrollReportViewModel>(viewResult.Model);
            Assert.Equal("MonthlyPayroll", viewResult.ViewName);
            Assert.Equal(viewModel.SelectedTime, viewResultModel.SelectedTime);
            Assert.Equal(viewModel.CompanyId, viewResultModel.CompanyId);
            Assert.Equal(companySelectList, viewResultModel.Companies);
            Assert.Equal(Message.PayrollNotGenerated, viewResult.ViewData.ModelState["CommonError"].Errors.FirstOrDefault().ErrorMessage);
        }

        [Fact]
        public async Task MonthlyPayrollPost_ReportHasData_ReturnFileContentResult()
        {
            // Arrange
            byte[] bytes = Convert.FromBase64String("R0lGODlhAQABAIAAAAAAAAAAACH5BAAAAAAALAAAAAABAAEAAAICTAEAOw==");
            var report = new MonthlyPayrollReport
            {
                Employees = new List<EmployeeBasicInfo> { new EmployeeBasicInfo { EmployeeId = 2 } },
                Month = DateTime.Today
            };
            var company = new Company { Code = "OW", Name = "One Work" };
            var viewModel = new MonthlyPayrollReportViewModel
            {
                SelectedTime = DateTime.Today,
                CompanyId = 22
            };

            var webRootPath = "abc";
            var fileName = @"ExcelTemplates\MonthlyPayrollReport.xlsx";
            var templatePath = Path.Combine(webRootPath, fileName);
            var reportController = GetReportController();
            hostingEnvironment.WebRootPath.Returns(webRootPath);
            baseService.GetDataEntityAsync<Company>(Arg.Is<GetCompanyByIdCriteria>(x => x.CompanyId == viewModel.CompanyId)).Returns(company);
            sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.MonthlyPayrollReport, Policy.Read).Returns(true);
            reportService.GetDataForMonthlyPayrollReportAsync(viewModel.SelectedTime, viewModel.CompanyId).Returns(report);
            exportExcelService.ExportMonthlyPayrollReport(templatePath, report).Returns(bytes);

            // Act
            var actual = await reportController.MonthlyPayroll(viewModel);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(actual);
            Assert.Equal(bytes, fileResult.FileContents);
            Assert.Equal(MimeType.XLSX, fileResult.ContentType);
            Assert.Equal($"Monthly_Payroll_Report_{report.Month.ToString("MM-yyyy")}_{report.Company.Code}.xlsx", fileResult.FileDownloadName);
        }

        #endregion MonthlyPayroll

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
            var reportController = GetReportController();
            commonService.GetSelectList(Arg.Any<IEnumerable<Company>>(), Arg.Any<string>(), Arg.Any<string>()).Returns(companySelectList);
            baseService.GetDataAsync<MultiSelectViewModel>(Arg.Any<GetEmployeeStatusByUserCriteria>()).Returns(employeeStatuses);

            // Act
            var actual = await reportController.DynamicReport();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsType<GenerateReportViewModel>(viewResult.Model);
            Assert.Equal("DynamicReport", viewResult.ViewName);
            Assert.Equal("1,2", viewModel.EmployeeStatusIds);
            Assert.Equal(companySelectList, viewModel.Companies);
        }

        [Fact]
        public async Task DynamicReport_ValidModel_ReturnFile()
        {
            // Arrange
            var viewModel = new GenerateReportViewModel
            {
                CompanyId = 1,
                EmployeeStatusIds = "1,2",
                SelectedIds = "1"
            };

            var employeeDatas = new List<DynamicReportModel>
            {
                new DynamicReportModel
                {
                    IDNo = "DAR001"
                }
            };

            var reportController = GetReportController();
            sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.DynamicReport, Policy.Read).Returns(true);
            reportService.GetEmployeeDataForDynamicReport(viewModel.CompanyId, viewModel.EmployeeStatusIds).Returns(employeeDatas);
            exportExcelService.ExportDynamicReport(employeeDatas, viewModel.SelectedIds.Split(',').Select(int.Parse)).Returns(new byte[10]);

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
                SelectedIds = "1"
            };

            var reportController = GetReportController();
            reportController.ModelState.AddModelError(nameof(viewModel.CompanyId), "Missing company id");
            sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.DynamicReport, Policy.Read).Returns(true);
            commonService.GetSelectList(Arg.Any<IEnumerable<Company>>(), Arg.Any<string>(), Arg.Any<string>()).Returns(companySelectList);

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
            var tuple = new Tuple<string, string>("1234", "12345");
            service.GenerateKey().Returns(tuple);

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