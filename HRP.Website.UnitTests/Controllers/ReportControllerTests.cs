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
        public async Task GetBankInReport_InValidParams_ReturnNull()
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
            Assert.Null(actual);
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

            var bankInData = new List<BankIn>
            {
                new BankIn { Salary = "1" },
                new BankIn { Salary = "1" },
            };

            var reportController = GetReportController();
            reportService.GetBankInAsync(Arg.Any<int>(), Arg.Any<DateTime>()).Returns(bankInData);

            //Act
            var actual = await reportController.GetBankInReport(bankInReportViewModel);

            //Assert
            Assert.IsType<FileContentResult>(actual);
        }

        [Fact]
        public async Task BasicSalaryDirectBankingAndCash_WhenCalled_ReturnExpectedView()
        {
            //Arrange
            var reportController = GetReportController();

            //Act
            var actual = await reportController.BasicSalaryDirectBankingAndCash();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<BasicReportViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
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