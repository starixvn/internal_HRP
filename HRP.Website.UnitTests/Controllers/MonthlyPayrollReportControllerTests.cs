namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Constants;
    using Core.Enumerations;
    using Criteria;
    using HRP.Core.Services;
    using HRP.Services;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Models;
    using Models.Employee;
    using Models.Report;
    using NSubstitute;
    using Resources;
    using Xunit;

    public class MonthlyPayrollReportControllerTests
    {
        private ISessionService sessionService;
        private IBaseService baseService;
        private IReportService reportService;
        private IHostingEnvironment hostingEnvironment;
        private IExportMonthlyPayrollReportService excelService;
        private IRijndaelService rijndaelService;
        private ICommonService commonService;

        [Fact]
        public async Task MonthlyPayroll_WhenCalled_ReturnAsExpectedViewResult()
        {
            // Arrange
            var rijndaelKeyValuePair = new RijndaelKeyValuePair
            {
                Key = "key",
                Value = "value"
            };
            var tuple = (rijndaelKeyValuePair, rijndaelKeyValuePair);
            var companies = new List<Company> { new Company { CompanyId = 1, Name = "DAR" } };
            var companySelectList = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));
            var controller = GetController();
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            commonService.GetSelectList(Arg.Any<IEnumerable<Company>>(), nameof(Company.CompanyId), nameof(Company.Code))
                        .Returns(companySelectList);
            rijndaelService.GenerateRijndaelKeyValuePair().Returns(tuple);

            // Act
            var actual = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewResultModel = Assert.IsType<MonthlyPayrollReportViewModel>(viewResult.Model);
            Assert.Equal("~/Views/Report/MonthlyPayroll.cshtml", viewResult.ViewName);
            Assert.Equal(companySelectList, viewResultModel.Companies);
            Assert.True(string.IsNullOrEmpty(viewResultModel.FilePassword));
        }

        [Fact]
        public async Task MonthlyPayrollPost_ModelStateIsInvalid_ReturnMonthlyPayrollViewResult()
        {
            // Arrange
            var viewModel = new MonthlyPayrollReportViewModel
            {
                SelectedTime = DateTime.Today,
                CompanyId = 22,
                CommonError = "ahihi",
                RijndaelKey = new RijndaelKeyValuePair { Key = "key" },
                RijndaelIV = new RijndaelKeyValuePair { Key = "key" }
            };
            var rijndaelKeyValuePair = new RijndaelKeyValuePair
            {
                Key = "key",
                Value = "value"
            };
            var tuple = (rijndaelKeyValuePair, rijndaelKeyValuePair);
            var companies = new List<Company> { new Company { CompanyId = 1, Name = "DAR" } };
            var companySelectList = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));
            var controller = GetController();
            commonService.GetSelectList(Arg.Any<IEnumerable<Company>>(), nameof(Company.CompanyId), nameof(Company.Code))
                        .Returns(companySelectList);
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), FunctionKey.MonthlyPayrollReport, Policy.Read).Returns(true);
            controller.ObjectValidator = Substitute.For<IObjectModelValidator>();
            controller.ObjectValidator.When(x => x.Validate(Arg.Any<ActionContext>(), Arg.Any<ValidationStateDictionary>(),
                Arg.Any<string>(), Arg.Any<Object>())).Do(x => controller.ModelState.AddModelError(nameof(viewModel.FilePassword), "Password is required"));
            rijndaelService.GenerateRijndaelKeyValuePair().Returns(tuple);

            // Act
            var actual = await controller.Index(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewResultModel = Assert.IsType<MonthlyPayrollReportViewModel>(viewResult.Model);
            Assert.Equal("~/Views/Report/MonthlyPayroll.cshtml", viewResult.ViewName);
            Assert.Equal(viewModel.SelectedTime, viewResultModel.SelectedTime);
            Assert.Equal(viewModel.CompanyId, viewResultModel.CompanyId);
            Assert.Equal(companySelectList, viewResultModel.Companies);
            Assert.Equal("Password is required", viewResult.ViewData.ModelState[nameof(viewModel.FilePassword)].Errors.FirstOrDefault().ErrorMessage);
        }

        [Fact]
        public async Task MonthlyPayrollPost_UserNotHasPermissionOnThisCompany_RedirectToAccessDenied()
        {
            // Arrange
            var viewModel = new MonthlyPayrollReportViewModel
            {
                SelectedTime = DateTime.Today,
                CompanyId = 22,
                RijndaelKey = new RijndaelKeyValuePair { Key = "key" },
                RijndaelIV = new RijndaelKeyValuePair { Key = "key" }
            };
            var controller = GetController();
            controller.ObjectValidator = Substitute.For<IObjectModelValidator>();
            controller.ObjectValidator.When(x => x.Validate(Arg.Any<ActionContext>(), Arg.Any<ValidationStateDictionary>(), Arg.Any<string>(), Arg.Any<Object>()));
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), FunctionKey.MonthlyPayrollReport, Policy.Read).Returns(false);

            // Act
            var actual = await controller.Index(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("AccessDenied", redirectResult.ActionName);
            Assert.Equal("Error", redirectResult.ControllerName);
        }

        [Fact]
        public async Task MonthlyPayrollPost_ReportNotHasAnyEmployees_ReturnMonthlyPayrollViewResult()
        {
            // Arrange
            var rijndaelKeyValuePair = new RijndaelKeyValuePair
            {
                Key = "key",
                Value = "value"
            };
            var tuple = (rijndaelKeyValuePair, rijndaelKeyValuePair);
            var report = new MonthlyPayrollReport
            {
                Employees = new List<EmployeeBasicInfo>(),
                Month = DateTime.Today
            };
            var viewModel = new MonthlyPayrollReportViewModel
            {
                SelectedTime = DateTime.Today,
                CompanyId = 22,
                RijndaelKey = new RijndaelKeyValuePair { Key = "key" },
                RijndaelIV = new RijndaelKeyValuePair { Key = "key" }
            };
            var companies = new List<Company> { new Company { CompanyId = 1, Name = "DAR" } };
            var companySelectList = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));

            var controller = GetController();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), FunctionKey.MonthlyPayrollReport, Policy.Read).Returns(true);
            reportService.GetDataForMonthlyPayrollReportAsync(viewModel.SelectedTime, Arg.Any<int>(), Arg.Any<int>()).Returns(report);
            commonService.GetSelectList(Arg.Any<IEnumerable<Company>>(), nameof(Company.CompanyId), nameof(Company.Code))
                        .Returns(companySelectList);
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), FunctionKey.MonthlyPayrollReport, Policy.Read).Returns(true);
            controller.ObjectValidator = Substitute.For<IObjectModelValidator>();
            controller.ObjectValidator.When(x => x.Validate(Arg.Any<ActionContext>(), Arg.Any<ValidationStateDictionary>(), Arg.Any<string>(), Arg.Any<Object>()));
            rijndaelService.GenerateRijndaelKeyValuePair().Returns(tuple);

            // Act
            var actual = await controller.Index(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewResultModel = Assert.IsType<MonthlyPayrollReportViewModel>(viewResult.Model);
            Assert.Equal("~/Views/Report/MonthlyPayroll.cshtml", viewResult.ViewName);
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
                CompanyId = 22,
                ReusePassword = true,
                EncryptedPassword = "123",
                RijndaelKey = new RijndaelKeyValuePair { Key = "key" },
                RijndaelIV = new RijndaelKeyValuePair { Key = "key" }
            };

            var expectedPassword = "456";

            var webRootPath = "abc";
            var fileName = @"ExcelTemplates/MonthlyPayrollReport.xlsx";
            var templatePath = Path.Combine(webRootPath, fileName);
            var controller = GetController();
            hostingEnvironment.WebRootPath.Returns(webRootPath);
            baseService.GetDataEntityAsync<Company>(Arg.Is<GetCompanyByIdCriteria>(x => x.CompanyId == viewModel.CompanyId)).Returns(company);
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), FunctionKey.MonthlyPayrollReport, Policy.Read).Returns(true);
            reportService.GetDataForMonthlyPayrollReportAsync(viewModel.SelectedTime, Arg.Any<int>(), Arg.Any<int>()).Returns(report);
            excelService.Export(templatePath, report, expectedPassword).Returns(bytes);
            controller.ObjectValidator = Substitute.For<IObjectModelValidator>();
            controller.ObjectValidator.When(x => x.Validate(Arg.Any<ActionContext>(), Arg.Any<ValidationStateDictionary>(), Arg.Any<string>(), Arg.Any<Object>()));
            rijndaelService.Decrypt(viewModel.EncryptedPassword, Arg.Any<string>(), Arg.Any<string>()).Returns(expectedPassword);

            // Act
            var actual = await controller.Index(viewModel);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(actual);
            Assert.Equal(bytes, fileResult.FileContents);
            Assert.Equal(MimeType.XLSX, fileResult.ContentType);
            Assert.Equal($"Monthly_Payroll_Report_{report.Month.ToString("MM-yyyy")}_{report.Company.Code}.xlsx", fileResult.FileDownloadName);
        }

        #region Helpers

        private static IRijndaelService FakeRijndaelService()
        {
            var service = Substitute.For<IRijndaelService>();
            var tuple = (new RijndaelKeyValuePair(), new RijndaelKeyValuePair());
            service.GenerateRijndaelKeyValuePair().Returns(tuple);

            return service;
        }

        private MonthlyPayrollReportController GetController()
        {
            sessionService = Substitute.For<ISessionService>();
            baseService = Substitute.For<IBaseService>();
            reportService = Substitute.For<IReportService>();
            excelService = Substitute.For<IExportMonthlyPayrollReportService>();
            hostingEnvironment = Substitute.For<IHostingEnvironment>();
            rijndaelService = FakeRijndaelService();
            commonService = Substitute.For<ICommonService>();

            var controller = new MonthlyPayrollReportController(
                baseService,
                sessionService,
                hostingEnvironment,
                excelService,
                reportService,
                rijndaelService,
                commonService)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>())
            };
            return controller;
        }

        #endregion Helpers
    }
}