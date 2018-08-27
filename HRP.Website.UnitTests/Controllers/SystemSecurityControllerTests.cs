namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.RefResource;
    using HRP.Models;
    using HRP.Models.SystemSecurity;
    using HRP.Services;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Routing;
    using NSubstitute;
    using Xunit;

    public class SystemSecurityControllerTests
    {
        private ISessionService sessionService;
        private IBaseService baseService;
        private ICertificateService certificateService;
        private IRijndaelService rijndaelService;

        [Fact]
        public void Index_WhenCalled_ReturnAsExpectedViewResult()
        {
            // Arrange
            var controller = GetSystemSecurityController();
            sessionService.HasPermissionOnFunction(FunctionKey.SecuritySetting, Policy.Host).Returns(true);

            // Act
            var actual = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<ChangeCertificatePasswordViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
            Assert.Null(viewModel.OldPassword);
            Assert.Null(viewModel.Password);
            Assert.Null(viewModel.ConfirmPassword);
            Assert.True(viewModel.IsHost);
            Assert.False(viewModel.IsSuccessful);
            Assert.Null(viewModel.MessageResult);
        }

        #region Generate

        [Fact]
        public void Generate_WhenCalled_ReturnAsExpectedViewResult()
        {
            // Arrange
            var rijndaelKeyValuePair = new RijndaelKeyValuePair
            {
                Key = "key",
                Value = "value"
            };
            var tuple = new Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair>(rijndaelKeyValuePair, rijndaelKeyValuePair);
            var controller = GetSystemSecurityController();
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            sessionService.HasPermissionOnFunction(FunctionKey.SecuritySetting, Policy.Host).Returns(true);
            rijndaelService.GenerateRijndaelKeyValuePair().Returns(tuple);

            // Act
            var actual = controller.Generate();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<GenerateCertificateViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
            Assert.Null(viewModel.Password);
            Assert.True(viewModel.IsHost);
            Assert.False(viewModel.IsSuccessful);
            Assert.Null(viewModel.MessageResult);
            Assert.Equal(viewModel.RijndaelKey, tuple.Item1);
            Assert.Equal(viewModel.RijndaelIV, tuple.Item2);
        }

        [Fact]
        public async Task GeneratePost_ModelStateIsInvalid_ReturnAsExpectedViewResult()
        {
            // Arrange
            var viewModel = new GenerateCertificateViewModel();
            var rijndaelKeyValuePair = new RijndaelKeyValuePair
            {
                Key = "key",
                Value = "value"
            };
            var tuple = new Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair>(rijndaelKeyValuePair, rijndaelKeyValuePair);
            var controller = GetSystemSecurityController();
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            controller.ModelState.AddModelError("password", "required");
            rijndaelService.GenerateRijndaelKeyValuePair().Returns(tuple);

            // Act
            var actual = await controller.Generate(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModelResult = Assert.IsAssignableFrom<GenerateCertificateViewModel>(viewResult.Model);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.True(viewResult.ViewData.ModelState.Count == 1, "required");
            Assert.Equal(viewModel.Password, viewModelResult.Password);
            Assert.Equal(viewModel.IsHost, viewModelResult.IsHost);
            Assert.Equal(viewModel.IsSuccessful, viewModelResult.IsSuccessful);
            Assert.Equal(viewModel.MessageResult, viewModelResult.MessageResult);
            Assert.Equal(viewModel.RijndaelKey, tuple.Item1);
            Assert.Equal(viewModel.RijndaelIV, tuple.Item2);
        }

        [Fact]
        public async Task GeneratePost_GenerateUnsuccessfully_ReturnAsExpectedViewResult()
        {
            // Arrange
            var password = "123";
            var commandResult = new CommandResult();
            var viewModel = new GenerateCertificateViewModel
            {
                RijndaelKey = new RijndaelKeyValuePair { Key = "key" },
                RijndaelIV = new RijndaelKeyValuePair { Key = "key" }
            };
            var rijndaelKeyValuePair = new RijndaelKeyValuePair
            {
                Key = "key",
                Value = "value"
            };
            var tuple = new Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair>(rijndaelKeyValuePair, rijndaelKeyValuePair);
            var controller = GetSystemSecurityController();
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            certificateService.GenerateSelfSignedCertificate(password).Returns(commandResult);
            rijndaelService.Decrypt(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(password);
            rijndaelService.GenerateRijndaelKeyValuePair().Returns(tuple);

            // Act
            var actual = await controller.Generate(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModelResult = Assert.IsAssignableFrom<GenerateCertificateViewModel>(viewResult.Model);
            Assert.Equal(password, viewModelResult.Password);
            Assert.Equal(viewModel.IsHost, viewModelResult.IsHost);
            Assert.Equal(commandResult.IsSuccessful, viewModelResult.IsSuccessful);
            Assert.Equal(commandResult.Message, viewModelResult.MessageResult);
            Assert.Equal(viewModel.RijndaelKey, tuple.Item1);
            Assert.Equal(viewModel.RijndaelIV, tuple.Item2);
        }

        [Fact]
        public async Task GeneratePost_GenerateSuccessfully_ReturnAsExpectedViewResult()
        {
            // Arrange
            var password = "123";
            var rijndaelKeyValuePair = new RijndaelKeyValuePair
            {
                Key = "key",
                Value = "value"
            };
            var tuple = new Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair>(rijndaelKeyValuePair, rijndaelKeyValuePair);
            var commandResult = new CommandResult { Code = 0 };
            var viewModel = new GenerateCertificateViewModel
            {
                RijndaelKey = new RijndaelKeyValuePair { Key = "key" },
                RijndaelIV = new RijndaelKeyValuePair { Key = "key" }
            };
            var controller = GetSystemSecurityController();
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            certificateService.GenerateSelfSignedCertificate(password).Returns(commandResult);
            rijndaelService.Decrypt(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(password);
            rijndaelService.GenerateRijndaelKeyValuePair().Returns(tuple);

            // Act
            var actual = await controller.Generate(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModelResult = Assert.IsAssignableFrom<GenerateCertificateViewModel>(viewResult.Model);
            Assert.Equal(password, viewModelResult.Password);
            Assert.Equal(viewModel.IsHost, viewModelResult.IsHost);
            Assert.Equal(commandResult.IsSuccessful, viewModelResult.IsSuccessful);
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
            var rijndaelKeyValuePair = new RijndaelKeyValuePair
            {
                Key = "key",
                Value = "value"
            };
            var tuple = new Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair>(rijndaelKeyValuePair, rijndaelKeyValuePair);
            var controller = GetSystemSecurityController();
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            sessionService.HasPermissionOnFunction(FunctionKey.SecuritySetting, Policy.Host).Returns(true);
            rijndaelService.GenerateRijndaelKeyValuePair().Returns(tuple);

            // Act
            var actual = controller.Password();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<InputPasswordViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
            Assert.Null(viewModel.Password);
            Assert.True(viewModel.IsHost);
            Assert.Equal(viewModel.RijndaelKey, tuple.Item1);
            Assert.Equal(viewModel.RijndaelIV, tuple.Item2);
        }

        [Fact]
        public void PasswordPost_ModelStateInvalid_ReturnAsExpectedViewResult()
        {
            // Arrange
            var viewModel = new InputPasswordViewModel();
            var rijndaelKeyValuePair = new RijndaelKeyValuePair
            {
                Key = "key",
                Value = "value"
            };
            var tuple = new Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair>(rijndaelKeyValuePair, rijndaelKeyValuePair);
            var controller = GetSystemSecurityController();
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            controller.ModelState.AddModelError("password", "required");
            rijndaelService.GenerateRijndaelKeyValuePair().Returns(tuple);

            // Act
            var actual = controller.Password(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModelResult = Assert.IsAssignableFrom<InputPasswordViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.True(viewResult.ViewData.ModelState.Count == 1);

            Assert.Equal(viewModel.Password, viewModelResult.Password);
            Assert.Equal(viewModel.IsHost, viewModelResult.IsHost);
            Assert.Equal(viewModel.RijndaelKey, tuple.Item1);
            Assert.Equal(viewModel.RijndaelIV, tuple.Item2);
        }

        [Fact]
        public void PasswordPost_IncorrectPassword_ReturnAsExpectedViewResult()
        {
            // Arrange
            var rijndaelKeyValuePair = new RijndaelKeyValuePair
            {
                Key = "key",
                Value = "value"
            };
            var tuple = new Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair>(rijndaelKeyValuePair, rijndaelKeyValuePair);
            var viewModel = new InputPasswordViewModel
            {
                RijndaelKey = new RijndaelKeyValuePair { Key = "key" },
                RijndaelIV = new RijndaelKeyValuePair { Key = "key" }
            };
            var controller = GetSystemSecurityController();
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            certificateService.IsCorrectPasswordAccessTheCertificate(tuple.Item1.Value).Returns(false);
            rijndaelService.Decrypt(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(tuple.Item1.Value);
            rijndaelService.GenerateRijndaelKeyValuePair().Returns(tuple);

            // Act
            var actual = controller.Password(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModelResult = Assert.IsAssignableFrom<InputPasswordViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.True(viewResult.ViewData.ModelState.Count == 1);

            Assert.Equal(tuple.Item1.Value, viewModelResult.Password);
            Assert.Equal(viewModel.RijndaelKey, tuple.Item1);
            Assert.Equal(viewModel.RijndaelIV, tuple.Item2);
        }

        [Fact]
        public void PasswordPost_CorrectPassword_ReturnRedirectToHome()
        {
            // Arrange
            var viewModel = new InputPasswordViewModel
            {
                RijndaelKey = new RijndaelKeyValuePair { Key = "key" },
                RijndaelIV = new RijndaelKeyValuePair { Key = "key" }
            };
            var password = "1234";
            var controller = GetSystemSecurityController();
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            certificateService.IsCorrectPasswordAccessTheCertificate(password).Returns(true);
            rijndaelService.Decrypt(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(password);

            // Act
            var actual = controller.Password(viewModel);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Home", viewResult.ControllerName);
            Assert.Equal("Index", viewResult.ActionName);

            certificateService.Received(1).CachePasswordAccessTheCertificate(viewModel.Password);
        }

        #endregion Password

        #region ChangePassword

        [Fact]
        public void ChangePasswordPost_ModelStateInvalid_ReturnAsExpectedErrors()
        {
            // Arrange
            var viewModel = new ChangeCertificatePasswordViewModel();
            var controller = GetSystemSecurityController();
            controller.ModelState.AddModelError("password", "required");

            // Act
            var actual = controller.ChangePassword(viewModel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var errors = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value.GetType().GetProperty("errors").GetValue(jsonResult.Value));
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
            Assert.Collection(
                errors,
                kvp =>
                {
                    Assert.Equal("password", kvp.Key);
                    Assert.Equal("required", kvp.Value);
                }
            );
        }

        [Fact]
        public void ChangePasswordPost_IncorrectPassword_ReturnAsExpectedErrors()
        {
            // Arrange
            var viewModel = new ChangeCertificatePasswordViewModel();
            var controller = GetSystemSecurityController();
            certificateService.IsCorrectPasswordAccessTheCertificate(viewModel.Password).Returns(false);

            // Act
            var actual = controller.ChangePassword(viewModel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var errors = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value.GetType().GetProperty("errors").GetValue(jsonResult.Value));
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
            Assert.Collection(
                errors,
                kvp =>
                {
                    Assert.Equal(nameof(viewModel.OldPassword), kvp.Key);
                    Assert.Equal("The old password you entered is incorrect.", kvp.Value);
                }
            );
        }

        [Fact]
        public void ChangePasswordPost_Unsuccessfully_ReturnAsExpected()
        {
            // Arrange
            var commandResult = new CommandResult();
            var viewModel = new ChangeCertificatePasswordViewModel();
            var controller = GetSystemSecurityController();
            certificateService.IsCorrectPasswordAccessTheCertificate(viewModel.Password).Returns(true);
            certificateService.ChangePasswordAccessTheCertificate(viewModel.OldPassword, viewModel.Password).Returns(commandResult);

            // Act
            var actual = controller.ChangePassword(viewModel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(commandResult, jsonResult.Value);
        }

        [Fact]
        public void ChangePasswordPost_Successfully_ReturnAsExpected()
        {
            // Arrange
            var commandResult = new CommandResult { Code = 0 };
            var viewModel = new ChangeCertificatePasswordViewModel();
            var controller = GetSystemSecurityController();
            certificateService.IsCorrectPasswordAccessTheCertificate(viewModel.Password).Returns(true);
            certificateService.ChangePasswordAccessTheCertificate(viewModel.OldPassword, viewModel.Password).Returns(commandResult);

            // Act
            var actual = controller.ChangePassword(viewModel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(commandResult, jsonResult.Value);

            certificateService.Received(1).CachePasswordAccessTheCertificate(viewModel.Password);
        }

        #endregion ChangePassword

        [Fact]
        public void Download_WhenCalled_ReturnAsExpectedFile()
        {
            // Arrange
            var controller = GetSystemSecurityController();
            certificateService.CopyCertificateFileToByteArray().Returns(new byte[] { 0xFF, 0x00 });

            // Act
            var actual = controller.Download();

            // Assert
            var fileStream = Assert.IsType<FileContentResult>(actual);
            Assert.Equal(MimeType.PFX, fileStream.ContentType);
            Assert.Equal("SecretKey.pfx", fileStream.FileDownloadName);
        }

        [Fact]
        public void FileNotFound_WhenCalled_ReturnAsExpectedViewResult()
        {
            // Arrange
            var controller = GetSystemSecurityController();
            sessionService.HasPermissionOnFunction(FunctionKey.SecuritySetting, Policy.Host).Returns(true);

            // Act
            var actual = controller.FileNotFound();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
            Assert.Equal(true, viewResult.ViewData["IsHost"]);
        }

        #region Helpers

        private static ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId = 1;

            return service;
        }

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private static ICertificateService FakeCertificateService()
        {
            var service = Substitute.For<ICertificateService>();

            return service;
        }

        private static IRijndaelService FakeRijndaelService()
        {
            var service = Substitute.For<IRijndaelService>();

            return service;
        }

        private SystemSecurityController GetSystemSecurityController()
        {
            sessionService = FakeSessionService();
            baseService = FakeBaseService();
            certificateService = FakeCertificateService();
            rijndaelService = FakeRijndaelService();
            var controller = new SystemSecurityController(sessionService, baseService, certificateService, rijndaelService);

            return controller;
        }

        #endregion Helpers
    }
}