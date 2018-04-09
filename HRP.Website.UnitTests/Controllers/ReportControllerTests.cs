namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Core.Services;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models.Report;
    using NSubstitute;
    using Services;
    using Website.Controllers;
    using Xunit;

    public class ReportControllerTests
    {
        private ISessionService sessionService;
        private IBaseService baseService;
        private IPaymentProcessService paymentProcessService;
        private IReportService reportService;
        private IHostingEnvironment hostingEnvironment;

        [Fact]
        public async Task BankIn_WhenCalled_ReturnExpectedView()
        {
            //Arrange
            var reportController = GetReportController();

            //Act
            var actual = await reportController.BankIn();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<BasicReportViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void BankIn_WhenPost_ReturnEmptyView()
        {
            //Arrange
            var reportController = GetReportController();
            reportController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var basicReportViewModel = new BasicReportViewModel
            {
                CompanyId = 1,
                Month = DateTime.Now
            };

            //Act
            var actual = reportController.BankIn(basicReportViewModel);

            //Assert
            Assert.IsType<EmptyResult>(actual);
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

        private ReportController GetReportController()
        {
            sessionService = FakeSessionService();
            baseService = FakeBaseService();
            paymentProcessService = FakePaymentProcessService();
            reportService = FakeReportService();
            hostingEnvironment = FakeHostingEnviroment();

            return new ReportController(
                baseService,
                sessionService,
                paymentProcessService,
                hostingEnvironment,
                reportService);
        }

        #endregion Helpers
    }
}