namespace HRP.Website.UnitTests.Controllers
{
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.RefResource;
    using HRP.Models.Certificate;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;

    public class CertificateControllerTests
    {
        private ISessionService sessionService;
        private IBaseService baseService;
        private IZipFileWrapper zipFileWrapper;
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

            // Act
            var actual = await controller.Generate(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModelResult = Assert.IsAssignableFrom<GenerateCertificateViewModel>(viewResult.Model);
            Assert.Equal(viewModel, viewModelResult);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.True(viewResult.ViewData.ModelState.Count == 1, "required");
        }

        [Fact]
        public async Task GeneratePost_GenerateUnsuccessfully_ReturnAsExpectedViewResult()
        {
            // Arrange
            var commandResult = new CommandResult();
            var viewModel = new GenerateCertificateViewModel();
            var controller = GetCertificateController();
            certificateService.GenerateSelfSignedCertificate(viewModel.Password).Returns(commandResult);

            // Act
            var actual = await controller.Generate(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModelResult = Assert.IsAssignableFrom<GenerateCertificateViewModel>(viewResult.Model);
            Assert.Equal(viewModel.Password, viewModelResult.Password);
            Assert.Equal(viewModel.IsSystemAdmin, viewModelResult.IsSystemAdmin);
            Assert.Equal(commandResult.IsSuccessful, viewModelResult.IsIsSuccessful);
            Assert.Equal(commandResult.Message, viewModelResult.MessageResult);
        }

        [Fact]
        public async Task GeneratePost_GenerateSuccessfully_ReturnAsExpectedViewResult()
        {
            // Arrange
            var commandResult = new CommandResult { Code = 0 };
            var viewModel = new GenerateCertificateViewModel();
            var controller = GetCertificateController();
            certificateService.GenerateSelfSignedCertificate(viewModel.Password).Returns(commandResult);

            // Act
            var actual = await controller.Generate(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModelResult = Assert.IsAssignableFrom<GenerateCertificateViewModel>(viewResult.Model);
            Assert.Equal(viewModel.Password, viewModelResult.Password);
            Assert.Equal(viewModel.IsSystemAdmin, viewModelResult.IsSystemAdmin);
            Assert.Equal(commandResult.IsSuccessful, viewModelResult.IsIsSuccessful);
            Assert.Null(viewModelResult.MessageResult);

            certificateService.Received(1).CachePasswordAccessTheCertificate(viewModel.Password);
            await baseService.Received(1).ExecuteNonQueryAsync(Arg.Any<UpdateRefResourceItemCriteria>());
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

        [Fact]
        public void PasswordPost_ModelStateInvalid_ReturnAsExpectedViewResult()
        {
            // Arrange
            var viewModel = new GenerateCertificateViewModel();
            var controller = GetCertificateController();
            controller.ModelState.AddModelError("password", "required");

            // Act
            var actual = controller.Password(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModelResult = Assert.IsAssignableFrom<GenerateCertificateViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.True(viewResult.ViewData.ModelState.Count == 1);

            Assert.Equal(viewModel, viewModelResult);
        }

        [Fact]
        public void PasswordPost_IncorrectPassword_ReturnAsExpectedViewResult()
        {
            // Arrange
            var viewModel = new GenerateCertificateViewModel();
            var controller = GetCertificateController();
            certificateService.IsCorrectPasswordAccessTheCertificate(viewModel.Password).Returns(false);

            // Act
            var actual = controller.Password(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModelResult = Assert.IsAssignableFrom<GenerateCertificateViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.True(viewResult.ViewData.ModelState.Count == 1);

            Assert.Equal(viewModel, viewModelResult);
        }

        [Fact]
        public void PasswordPost_CorrectPassword_ReturnRedirectToHome()
        {
            // Arrange
            var viewModel = new GenerateCertificateViewModel();
            var controller = GetCertificateController();
            certificateService.IsCorrectPasswordAccessTheCertificate(viewModel.Password).Returns(true);

            // Act
            var actual = controller.Password(viewModel);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Home", viewResult.ControllerName);
            Assert.Equal("Index", viewResult.ActionName);

            certificateService.Received(1).CachePasswordAccessTheCertificate(viewModel.Password);
        }

        #endregion Password

        [Fact]
        public void Download_WhenCalled_ReturnAsExpectedFile()
        {
            // Arrange
            var sourceFileName = "D:\\HRP.pfx";
            var entryName = "HRP.pfx";
            var controller = GetCertificateController();
            certificateService.GetCertificateFilePath().Returns(sourceFileName);
            zipFileWrapper.CopyFileToByteArray(sourceFileName, entryName).Returns(new byte[] { 0xFF, 0x00 });

            // Act
            var actual = controller.Download();

            // Assert
            var fileStream = Assert.IsType<FileContentResult>(actual);
            Assert.Equal(MimeType.ZIP, fileStream.ContentType);
            Assert.Equal("HRP.zip", fileStream.FileDownloadName);
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

        private IZipFileWrapper FakeZipFileWrapper()
        {
            return Substitute.For<IZipFileWrapper>();
        }

        private ICertificateService FakeCertificateService()
        {
            var service = Substitute.For<ICertificateService>();

            return service;
        }

        private CertificateController GetCertificateController()
        {
            sessionService = FakeSessionService();
            baseService = FakeBaseService();
            zipFileWrapper = FakeZipFileWrapper();
            certificateService = FakeCertificateService();
            var controller = new CertificateController(sessionService, baseService, zipFileWrapper, certificateService);

            return controller;
        }

        #endregion Helpers
    }
}