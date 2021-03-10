namespace HRP.Services.UnitTests
{
    using System.Threading.Tasks;
    using AutoFixture;
    using BCrypt.Net;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using HRP.Models.MailSetting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using Models;
    using NSubstitute;
    using Services;
    using Xunit;

    public class UserServiceTests
    {
        private readonly IFixture fixture = new Fixture();
        private IHttpContextAccessor httpContextAccessor;
        private IBaseMailService emailService;
        private IBaseService baseService;
        private IMailSettingService mailSettingService;

        #region GenerateTokenOTPRegister

        [Fact]
        public async Task GenerateTokenOTPRegister_UpdateSuccessful_ReturnExpectedToken()
        {
            // Arrange
            var commandResult = new CommandResult
            {
                Code = 0
            };
            var criteria = new UpdateTokenOTPRegisterCriteria();
            var userService = GetUserService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateTokenOTPRegisterCriteria>(x => criteria = x)).Returns(commandResult);

            // Act
            var token = await userService.GenerateTokenOTPRegisterAsync("test");

            // Assert
            Assert.Equal("test", criteria.Username);
            Assert.NotNull(token);
        }

        [Fact]
        public async Task GenerateTokenOTPRegister_UpdateUnsuccessful_ReturnNull()
        {
            // Arrange
            var commandResult = new CommandResult
            {
                Code = 1
            };
            var criteria = new UpdateTokenOTPRegisterCriteria();
            var userService = GetUserService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateTokenOTPRegisterCriteria>(x => criteria = x)).Returns(commandResult);

            // Act
            var token = await userService.GenerateTokenOTPRegisterAsync("test");

            // Assert
            Assert.Equal("test", criteria.Username);
            Assert.Null(token);
        }

        #endregion GenerateTokenOTPRegister

        [Fact]
        public async Task GetUserByUsername_WhenCalled_ReturnExpectedUser()
        {
            // Arrange
            var expected = fixture.Create<User>();
            var criteria = new GetUserByUsernameCriteria();
            var userService = GetUserService();
            baseService.GetDataEntityAsync<User>(Arg.Do<GetUserByUsernameCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await userService.GetUserByUsernameAsync("test");

            // Assert
            Assert.Equal("test", criteria.Username);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetUserByTokenOTPRegister_WhenCalled_ReturnExpectedUser()
        {
            // Arrange
            var expected = fixture.Create<User>();
            var criteria = new GetUserByTokenOTPRegisterCriteria();
            var userService = GetUserService();
            baseService.GetDataEntityAsync<User>(Arg.Do<GetUserByTokenOTPRegisterCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await userService.GetUserByTokenOTPRegisterAsync("token");

            // Assert
            Assert.Equal("token", criteria.TokenOTPRegister);
            Assert.Equal(expected, actual);
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
        public async Task ResetTokenOTPRegister_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var commandResult = fixture.Create<CommandResult>();
            var criteria = new UpdateTokenOTPRegisterCriteria();
            var userService = GetUserService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateTokenOTPRegisterCriteria>(x => criteria = x)).Returns(commandResult);

            // Act
            var result = await userService.ResetTokenOTPRegisterAsync("test");

            // Assert
            Assert.Equal("test", criteria.Username);
            Assert.Null(criteria.TokenOTPRegister);
            Assert.Equal(commandResult.IsSuccessful, result);
        }

        #region ResetUserPassword

        [Fact]
        public async Task ResetUserPassword_UpdateSuccessful_ExpectedSendMail()
        {
            // Arrange
            var commandResult = new CommandResult
            {
                Code = 0
            };
            var userService = GetUserService();
            baseService.ExecuteNonQueryAsync(Arg.Any<UpdateTokenResetPasswordCriteria>()).Returns(commandResult);
            baseService.GetDataEntityAsync<User>(Arg.Any<GetUserByUsernameCriteria>()).Returns(new User());
            mailSettingService.GetMailSettingForSendMailAsync(Arg.Any<int>()).Returns(new MailSettingModel());

            // Act
            await userService.ResetUserPasswordAsync("test");

            // Assert
            await emailService.Received().SendEmailAsync(Arg.Any<MailModel>());
        }

        [Fact]
        public async Task ResetUserPassword_UpdateUnsuccessful_ExpectedNotSendMail()
        {
            // Arrange
            var commandResult = new CommandResult
            {
                Code = 1
            };
            var userService = GetUserService();
            baseService.ExecuteNonQueryAsync(Arg.Any<UpdateTokenResetPasswordCriteria>()).Returns(commandResult);

            // Act
            await userService.ResetUserPasswordAsync("test");

            // Assert
            await emailService.DidNotReceive().SendEmailAsync(Arg.Any<MailModel>());
        }

        #endregion ResetUserPassword

        [Fact]
        public async Task SendMailToRegisterOTP_WhenCalled_ExpectedSendMail()
        {
            // Arrange
            var userService = GetUserService();
            baseService.GetDataEntityAsync<User>(Arg.Any<GetUserByTokenOTPRegisterCriteria>()).Returns(new User());
            mailSettingService.GetMailSettingForSendMailAsync(Arg.Any<int>()).Returns(new MailSettingModel());

            // Act
            await userService.SendMailToRegisterOTPAsync("test");

            // Assert
            await emailService.Received().SendEmailAsync(Arg.Any<MailModel>());
        }

        [Fact]
        public async Task UpdateOTPStatus_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var commandResult = new CommandResult();
            var userService = GetUserService();
            baseService.ExecuteNonQueryAsync(Arg.Any<UpdateUserOTPStatusCriteria>()).Returns(commandResult);

            // Act
            var result = await userService.UpdateOTPStatusAsync(1, true);

            // Assert
            Assert.Equal(commandResult.IsSuccessful, result);
        }

        [Fact]
        public async Task UpdateSecretKey_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var commandResult = new CommandResult();
            var userService = GetUserService();
            baseService.ExecuteNonQueryAsync(Arg.Any<UpdateSecretKeyCriteria>()).Returns(commandResult);

            // Act
            var result = await userService.UpdateSecretKeyAsync("test", "test");

            // Assert
            Assert.Equal(commandResult.IsSuccessful, result);
        }

        #region Helpers

        private UserService GetUserService()
        {
            httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            baseService = Substitute.For<IBaseService>();
            emailService = Substitute.For<IBaseMailService>();
            mailSettingService = Substitute.For<IMailSettingService>();

            var appSettingsOptions = new AppSettingsOptions
            {
                SessionTimeout = 60,
                EnableEncrypt = true,
                Domain = "http=//localhost=56977"
            };

            var configuration = Substitute.For<IOptionsSnapshot<AppSettingsOptions>>();
            configuration.Value.Returns(appSettingsOptions);

            var service = new UserService(httpContextAccessor, emailService, baseService, mailSettingService);

            return service;
        }

        #endregion Helpers
    }
}