namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Core.Services;
    using HRP.Core.Constants;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Models.Report;
    using NSubstitute;
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
        private IRefResourceService refResourceService;

        [Fact]
        public async Task BankIn_WhenCalled_ReturnExpectedView()
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
        public async Task GetBankInReport_InValidParams_ReturnEmpty()
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
        public async Task GetBankInReport_ValidParams_ReturnNull()
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
            reportService.GetBankInAsync(Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<PaymentType>(), Arg.Any<string>()).Returns(bankInReport);

            //Act
            var actual = await reportController.GetDirectBankingReport(bankInReportViewModel);

            //Assert
            Assert.IsType<FileContentResult>(actual);
        }

        [Fact]
        public async Task GetMonthlyPayrollReport_WhenCalled_ReturnFileContentResult()
        {
            // Arrange
            byte[] bytes = Convert.FromBase64String("R0lGODlhAQABAIAAAAAAAAAAACH5BAAAAAAALAAAAAABAAEAAAICTAEAOw==");
            var report = new MonthlyPayrollReport();
            var month = DateTime.Today;
            var companyId = 22;
            var webRootPath = "abc";
            var fileName = @"ExcelTemplates\MonthlyPayrollReport.xlsx";
            var templatePath = Path.Combine(webRootPath, fileName);
            var reportController = GetReportController();
            hostingEnvironment.WebRootPath.Returns(webRootPath);
            reportService.GetDataForMonthlyPayrollReportAsync(Arg.Is<DateTime>(x => x == month), Arg.Is<int>(x => x == companyId)).Returns(report);
            exportExcelService.ExportMonthlyPayrollReport(Arg.Is<string>(x => x == templatePath), Arg.Is<MonthlyPayrollReport>(x => x == report)).Returns(bytes);

            // Act
            var actual = await reportController.GetMonthlyPayrollReport(month, companyId);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(actual);
            Assert.Equal(bytes, fileResult.FileContents);
            Assert.Equal(MimeType.XLSX, fileResult.ContentType);
            Assert.Equal($"MonthlyPayrollReport_{month.ToString("MM-yyyy")}_{companyId}.xlsx", fileResult.FileDownloadName);
        }

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

        private IRefResourceService FakeRefResourceService()
        {
            return Substitute.For<IRefResourceService>();
        }

        private ReportController GetReportController()
        {
            sessionService = FakeSessionService();
            baseService = FakeBaseService();
            reportService = FakeReportService();
            exportExcelService = FakeExportExcelService();
            hostingEnvironment = FakeHostingEnviroment();
            refResourceService = FakeRefResourceService();

            return new ReportController(
                baseService,
                sessionService,
                hostingEnvironment,
                exportExcelService,
                reportService,
                refResourceService);
        }

        #endregion Helpers
    }
}