namespace HRP.Website.UnitTests.Controllers
{
    using System.Threading.Tasks;
    using HRP.Core.Services;
    using HRP.Models.Certificate;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;

    public class CertificateControllerTests
    {
        private ISessionService sessionService;
        private ICertificateService certificateService;

        #region Generate

        [Fact]
        public void Generate_WhenCalled_ReturnAsExpectedViewResult()
        {
            // Arrange
            var controller = GetCertificateController();
            sessionService.IsSystemAdmin = true;

            // Act
            var actual = controller.Generate();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<GenerateCertificateViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
            Assert.Null(viewModel.Password);
            Assert.True(viewModel.IsSystemAdmin);
            Assert.False(viewModel.IsIsSuccessful);
            Assert.Null(viewModel.MessageResult);
        }

        [Fact]
        public async Task GeneratePost_ModelStateIsInvalid_ReturnAsExpectedViewResult()
        {
            // Arrange
            var viewModel = new GenerateCertificateViewModel();
            var controller = GetCertificateController();
            controller.ModelState.AddModelError("password", "required");
            sessionService.IsSystemAdmin = true;

            // Act
            var actual = await controller.Generate(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModelResult = Assert.IsAssignableFrom<GenerateCertificateViewModel>(viewResult.Model);
            Assert.Equal(viewModel, viewModelResult);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.True(viewResult.ViewData.ModelState.Count == 1, "required");
        }

        #endregion Generate

        #region Password

        [Fact]
        public void Password_WhenCalled_ReturnAsExpectedViewResult()
        {
            // Arrange
            var controller = GetCertificateController();
            sessionService.IsSystemAdmin = true;

            // Act
            var actual = controller.Password();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<GenerateCertificateViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
            Assert.Null(viewModel.Password);
            Assert.True(viewModel.IsSystemAdmin);
            Assert.False(viewModel.IsIsSuccessful);
            Assert.Null(viewModel.MessageResult);
        }

        #endregion Password

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

        private ICertificateService FakeCertificateService()
        {
            var service = Substitute.For<ICertificateService>();

            return service;
        }

        private CertificateController GetCertificateController()
        {
            sessionService = FakeSessionService();
            var baseService = FakeBaseService();
            certificateService = FakeCertificateService();
            var controller = new CertificateController(sessionService, baseService, certificateService);

            return controller;
        }

        #endregion Helpers
    }
}