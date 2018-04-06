namespace HRP.Website.UnitTests.Controllers
{
    using Core.Services;
    using HRP.Services;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Website.Controllers;
    using Xunit;

    public class ReportControllerTests
    {
        private ISessionService sessionService;
        private IBaseService baseService;
        private IPaymentProcessService paymentProcessService;

        [Fact]
        public void BankIn_WhenCalled_ReturnExpectedView()
        {
            //Arrange
            var reportController = GetReportController();

            //Act
            var actual = reportController.BankIn();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
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

        private ReportController GetReportController()
        {
            sessionService = FakeSessionService();
            baseService = FakeBaseService();
            paymentProcessService = FakePaymentProcessService();
            var controller = new ReportController(baseService, sessionService, paymentProcessService);

            return controller;
        }

        #endregion Helpers
    }
}