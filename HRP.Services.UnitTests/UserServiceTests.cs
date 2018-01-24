namespace HRP.Services.UnitTests
{
    using BCrypt.Net;
    using Core.Model;
    using Core.Services;
    using HRP.Criteria;
    using Microsoft.Extensions.Options;
    using Models;
    using NSubstitute;
    using Services;
    using Xunit;

    public class UserServiceTests
    {
        private IBaseMailService emailService;
        private IBaseService baseService;

        #region GenerateTokenOTPRegister

        [Fact]
        public void GenerateTokenOTPRegister_UpdateSuccessful_ReturnExpectedToken()
        {
            // Arrange
            var commandResult = new CommandResult
            {
                Code = 0
            };
            var userService = GetUserService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateTokenOTPRegisterCriteria>()).Returns(commandResult);

            // Act
            var token = userService.GenerateTokenOTPRegister("test");

            // Assert
            Assert.NotNull(token);
        }

        [Fact]
        public void GenerateTokenOTPRegister_UpdateUnsuccessful_ReturnNull()
        {
            // Arrange
            var commandResult = new CommandResult
            {
                Code = 1
            };
            var userService = GetUserService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateTokenOTPRegisterCriteria>()).Returns(commandResult);

            // Act
            var token = userService.GenerateTokenOTPRegister(Arg.Any<string>());

            // Assert
            Assert.Null(token);
        }

        #endregion GenerateTokenOTPRegister

        [Fact]
        public void GetUserByUsername_WhenCalled_ReturnExpectedUser()
        {
            // Arrange
            var userService = GetUserService();
            baseService.GetDataEntity<User>(Arg.Any<GetUserByUsernameCriteria>()).Returns(new User());

            // Act
            var user = userService.GetUserByUsername(Arg.Any<string>());

            // Assert
            Assert.IsType<User>(user);
        }

        [Fact]
        public void IsCorrectPassword_MatchPassword_ReturnTrue()
        {
            //Arrange
            var userService = GetUserService();
            var hashedPassword = BCrypt.HashPassword("test");

            //Act
            var verifyPassword = userService.IsCorrectPassword("test", hashedPassword);

            //Assert
            Assert.True(verifyPassword);
        }

        [Fact]
        public void IsCorrectPassword_NotMatchPassword_ReturnFalse()
        {
            //Arrange
            var userService = GetUserService();
            var hashedPassword = BCrypt.HashPassword("wrongPassword");

            //Act
            var verifyPassword = userService.IsCorrectPassword("test", hashedPassword);

            //Assert
            Assert.False(verifyPassword);
        }

        [Fact]
        public void ResetTokenOTPRegister_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var commandResult = new CommandResult();
            var userService = GetUserService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateTokenOTPRegisterCriteria>()).Returns(commandResult);

            // Act
            var result = userService.ResetTokenOTPRegister(Arg.Any<string>());

            // Assert
            Assert.Equal(commandResult.IsSuccessful, result);
        }

        #region ResetUserPassword

        [Fact]
        public void ResetUserPassword_UpdateSuccessful_ExpectedSendMail()
        {
            // Arrange
            var commandResult = new CommandResult
            {
                Code = 0
            };
            var userService = GetUserService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateTokenResetPasswordCriteria>()).Returns(commandResult);
            baseService.GetDataEntity<User>(Arg.Any<GetUserByUsernameCriteria>()).Returns(new User());

            // Act
            userService.ResetUserPassword("test");

            // Assert
            emailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        [Fact]
        public void ResetUserPassword_UpdateUnsuccessful_ExpectedNotSendMail()
        {
            // Arrange
            var commandResult = new CommandResult
            {
                Code = 1
            };
            var userService = GetUserService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateTokenResetPasswordCriteria>()).Returns(commandResult);

            // Act
            userService.ResetUserPassword("test");

            // Assert
            emailService.DidNotReceive().SendEmail(Arg.Any<MailModel>());
        }

        #endregion ResetUserPassword

        [Fact]
        public void SendMailToRegisterOTP_WhenCalled_ExpectedSendMail()
        {
            // Arrange
            var userService = GetUserService();
            baseService.GetDataEntity<User>(Arg.Any<GetUserByTokenOTPRegisterCriteria>()).Returns(new User());

            // Act
            userService.SendMailToRegisterOTP("test");

            // Assert
            emailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        [Fact]
        public void UpdateOTPStatus_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var commandResult = new CommandResult();
            var userService = GetUserService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateUserOTPStatusCriteria>()).Returns(commandResult);

            // Act
            var result = userService.UpdateOTPStatus(1, true);

            // Assert
            Assert.Equal(commandResult.IsSuccessful, result);
        }

        [Fact]
        public void UpdateSecretKey_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var commandResult = new CommandResult();
            var userService = GetUserService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateSecretKeyCriteria>()).Returns(commandResult);

            // Act
            var result = userService.UpdateSecretKey("test", "test");

            // Assert
            Assert.Equal(commandResult.IsSuccessful, result);
        }

        #region Helpers

        private UserService GetUserService()
        {
            baseService = Substitute.For<IBaseService>();
            emailService = Substitute.For<IBaseMailService>();

            var appSettingsOptions = new AppSettingsOptions
            {
                SessionTimeout = 60,
                EnableEncrypt = true,
                CertificateFilePath = "D=\\Test.pfx",
                PasswordToSignTheCertificateFile = "123456",
                BaseHostUrl = "http=//localhost=56977"
            };

            var configuration = Substitute.For<IOptionsSnapshot<AppSettingsOptions>>();
            configuration.Value.Returns(appSettingsOptions);

            var service = new UserService(emailService, configuration, baseService);

            return service;
        }

        #endregion Helpers
    }
}