namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Services;
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
        private IPaymentProcessService paymentProcessService;
        private IReportService reportService;
        private IHostingEnvironment hostingEnvironment;
        private IExportExcelService exportExcelService;

        [Fact]
        public async Task BankIn_WhenCalled_ReturnExpectedView()
        {
            //Arrange
            var reportController = GetReportController();

            //Act
            var actual = await reportController.BankIn();

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
            var actual = await reportController.GetBankInReport(bankInReportViewModel);

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
                DirectBanking1BankIns = bankInData,
                DirectBanking2BankIns = bankInData,
                CashBankIns = bankInData,
                CompanyName = companyName,
                ReportedMonth = reportedMonth
            };

            var reportController = GetReportController();
            reportService.GetBankInAsync(Arg.Any<int>(), Arg.Any<DateTime>()).Returns(bankInReport);

            //Act
            var actual = await reportController.GetBankInReport(bankInReportViewModel);

            //Assert
            Assert.IsType<FileContentResult>(actual);
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

        private IPaymentProcessService FakePaymentProcessService()
        {
            var service = Substitute.For<IPaymentProcessService>();

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

        private ReportController GetReportController()
        {
            sessionService = FakeSessionService();
            baseService = FakeBaseService();
            paymentProcessService = FakePaymentProcessService();
            reportService = FakeReportService();
            exportExcelService = FakeExportExcelService();
            hostingEnvironment = FakeHostingEnviroment();

            return new ReportController(
                baseService,
                sessionService,
                paymentProcessService,
                hostingEnvironment,
                exportExcelService,
                reportService);
        }

        #endregion Helpers
    }
}