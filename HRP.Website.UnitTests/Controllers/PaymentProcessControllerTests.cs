namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.PaymentProcess;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;

    public class PaymentProcessControllerTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private IHostingEnvironment hostingEnvironment;

        [Fact]
        public void Index_WhenCalled_ReturnExpectedViewResult()
        {
            var controller = GetPaymentProcessController();
            baseService.GetData<Company>(Arg.Any<GetAllCompanyCriteria>()).Returns(new List<Company>());

            var actual = controller.Index();

            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsType<SelectWorkingMonthCompanyViewModel>(viewResult.Model);
            Assert.Equal(DateTime.Today, viewModel.SelectedTime);
        }

        private PaymentProcessController GetPaymentProcessController()
        {
            FakeSessionService();
            FakeBaseService();
            FakeHostingEnvironment();

            return new PaymentProcessController(baseService, sessionService, hostingEnvironment);
        }

        private void FakeSessionService()
        {
            sessionService = Substitute.For<ISessionService>();
        }

        private void FakeBaseService()
        {
            baseService = Substitute.For<IBaseService>();
        }

        private void FakeHostingEnvironment()
        {
            hostingEnvironment = Substitute.For<IHostingEnvironment>();
        }
    }
}