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
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc;
    using Models.Employee;
    using Models.Report;
    using Models;
    using Newtonsoft.Json;
    using NSubstitute;
    using Resources;
    using Services.Interfaces;
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
        private ICommonService commonService;

        [Fact]
        public async Task GetEbankingPaymentTransaction_WhenCalled_ReturnEbanking()
        {
            //Arrange
            var reportController = GetReportController();

            var refResources = new List<ReferenceResourceModel>
            {
                new ReferenceResourceModel
                {
                    IsActive = true,
                    RefName = ReferenceResource.PaymentTransactionEbanking,
                    ResourceId = (int)BankTypes.Ebanking,
                    Value = ((int)BankTypes.Ebanking).ToString()
                }
            };

            var expected = new List<MultiSelectViewModel>();
            expected.AddRange(refResources.Select(x => new MultiSelectViewModel
            {
                Value = $"{(int)BankTypes.Ebanking}-{x.ResourceId}",
                Text = x.Value
            }));

            commonService.GetReferenceResourcesByNamesAsync(Arg.Any<string[]>()).Returns(refResources);

            //Act
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
            //Arrange
            var reportController = GetReportController();

            var refResources = new List<ReferenceResourceModel>
            {
                new ReferenceResourceModel
                {
                    IsActive = true,
                    RefName = ReferenceResource.PaymentTransactionDirectBanking,
                    ResourceId = 1,
                    Value = "1"
                },
                new ReferenceResourceModel
                {
                    IsActive = true,
                    RefName = ReferenceResource.PaymentTransactionCash,
                    ResourceId = 1,
                    Value = "1"
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

            //Act
            var actual = await reportController.GetDirectBankingPaymentTransactions();

            //Assert
            var json = Assert.IsType<JsonResult>(actual);
            Assert.Equal(
                JsonConvert.SerializeObject(json.Value),
                JsonConvert.SerializeObject(expected));
        }

        [Fact]
        public async Task DirectBanking_WhenCalled_ReturnExpectedView()
        {
            //Arrange
            var reportController = GetReportController();

            //Act
            var actual = await reportController.DirectBanking();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<BankInReportViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task Ebanking_WhenCalled_ReturnExpectedView()
        {
            //Arrange
            var reportController = GetReportController();

            //Act
            var actual = await reportController.Ebanking();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<BankInReportViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task GetDirectBankingReport_InValidParams_ReturnEmpty()
        {
            //Arrage
            var bankInReportViewModel = new BankInReportViewModel
            {
                CompanyId = 0,
                SelectedTime = DateTime.Now
            };

            var reportController = GetReportController();

            //Act
            var actual = await reportController.GetDirectBankingReport(bankInReportViewModel);

            //Assert
            Assert.IsType<RedirectToActionResult>(actual);
        }

        [Fact]
        public async Task GetDirectBankingReport_ValidParams_ReturnNull()
        {
            //Arrage
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

            //Act
            var actual = await reportController.GetDirectBankingReport(bankInReportViewModel);

            //Assert
            Assert.IsType<FileContentResult>(actual);
        }

        [Fact]
        public async Task GetEbankingReport_InValidParams_ReturnEmpty()
        {
            //Arrage
            var bankInReportViewModel = new BankInReportViewModel
            {
                CompanyId = 0,
                SelectedTime = DateTime.Now
            };

            var reportController = GetReportController();

            //Act
            var actual = await reportController.GetEbankingReport(bankInReportViewModel);

            //Assert
            Assert.IsType<RedirectToActionResult>(actual);
        }

        [Fact]
        public async Task GetEbankingReport_ValidParams_ReturnNull()
        {
            //Arrage
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

            //Act
            var actual = await reportController.GetEbankingReport(bankInReportViewModel);

            //Assert
            Assert.IsType<FileContentResult>(actual);
        }

        #region MonthlyPayroll

        [Fact]
        public async Task MonthlyPayroll_WhenCalled_ReturnMonthlyPayrollViewResult()
        {
            // Arrange
            var companies = new List<Company> { MockCompany() };
            var companySelectList = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));
            var reportController = GetReportController();
            reportController.ModelState.AddModelError("CompanyId", "Required");
            commonService.GetCompanyByFunctionKeyAsync(FunctionKey.Report).Returns(companies);
            commonService.GetSelectList(Arg.Is<IEnumerable<Company>>(x => x == companies), Arg.Is<string>(x => x == nameof(Company.CompanyId)), Arg.Is<string>(x => x == nameof(Company.Code)))
                        .Returns(companySelectList);

            // Act
            var actual = await reportController.MonthlyPayroll();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewResultModel = Assert.IsType<MonthlyPayrollReportViewModel>(viewResult.Model);
            Assert.Equal("MonthlyPayroll", viewResult.ViewName);
            Assert.Equal(DateTime.Today, viewResultModel.SelectedTime);
            Assert.Equal(companySelectList, viewResultModel.Companies);
        }

        [Fact]
        public async Task MonthlyPayrollPost_ModelStateIsInvalid_ReturnMonthlyPayrollViewResult()
        {
            // Arrange
            var viewModel = new MonthlyPayrollReportViewModel
            {
                SelectedTime = DateTime.Today,
                CompanyId = 22
            };

            var companies = new List<Company> { MockCompany() };
            var companySelectList = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));
            var reportController = GetReportController();
            reportController.ModelState.AddModelError("CompanyId", "Required");
            commonService.GetCompanyByFunctionKeyAsync(FunctionKey.Report).Returns(companies);
            commonService.GetSelectList(Arg.Is<IEnumerable<Company>>(x => x == companies), Arg.Is<string>(x => x == nameof(Company.CompanyId)), Arg.Is<string>(x => x == nameof(Company.Code)))
                        .Returns(companySelectList);

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
                Month = DateTime.Today,
                CompanyCode = "OW"
            };
            var viewModel = new MonthlyPayrollReportViewModel
            {
                SelectedTime = DateTime.Today,
                CompanyId = 22
            };
            var companies = new List<Company> { MockCompany() };
            var companySelectList = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));

            var reportController = GetReportController();
            reportService.GetDataForMonthlyPayrollReportAsync(Arg.Is<DateTime>(x => x == viewModel.SelectedTime), Arg.Is<int>(x => x == viewModel.CompanyId)).Returns(report);
            commonService.GetCompanyByFunctionKeyAsync(FunctionKey.Report).Returns(companies);
            commonService.GetSelectList(Arg.Is<IEnumerable<Company>>(x => x == companies), Arg.Is<string>(x => x == nameof(Company.CompanyId)), Arg.Is<string>(x => x == nameof(Company.Code)))
                        .Returns(companySelectList);

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
                Month = DateTime.Today,
                CompanyCode = "OW"
            };
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
            reportService.GetDataForMonthlyPayrollReportAsync(Arg.Is<DateTime>(x => x == viewModel.SelectedTime), Arg.Is<int>(x => x == viewModel.CompanyId)).Returns(report);
            exportExcelService.ExportMonthlyPayrollReport(Arg.Is<string>(x => x == templatePath), Arg.Is<MonthlyPayrollReport>(x => x == report)).Returns(bytes);

            // Act
            var actual = await reportController.MonthlyPayroll(viewModel);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(actual);
            Assert.Equal(bytes, fileResult.FileContents);
            Assert.Equal(MimeType.XLSX, fileResult.ContentType);
            Assert.Equal($"MonthlyPayrollReport_{report.Month.ToString("MM-yyyy")}_{report.CompanyCode}.xlsx", fileResult.FileDownloadName);
        }

        #endregion MonthlyPayroll

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId = 1;

            return service;
        }

        private IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private IReportService FakeReportService()
        {
            return Substitute.For<IReportService>();
        }

        private IHostingEnvironment FakeHostingEnviroment()
        {
            return Substitute.For<IHostingEnvironment>();
        }

        private IExportExcelService FakeExportExcelService()
        {
            return Substitute.For<IExportExcelService>();
        }

        private ICommonService FakeCommonService()
        {
            return Substitute.For<ICommonService>();
        }

        private ReportController GetReportController()
        {
            sessionService = FakeSessionService();
            baseService = FakeBaseService();
            reportService = FakeReportService();
            exportExcelService = FakeExportExcelService();
            hostingEnvironment = FakeHostingEnviroment();
            commonService = FakeCommonService();

            return new ReportController(
                baseService,
                sessionService,
                hostingEnvironment,
                exportExcelService,
                reportService,
                commonService);
        }

        private Company MockCompany()
        {
            return new Company
            {
                CompanyId = 11,
                Code = "company",
                Name = "descC",
                IsActive = true
            };
        }

        #endregion Helpers
    }
}