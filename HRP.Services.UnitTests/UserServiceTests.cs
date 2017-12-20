namespace HRP.Services.UnitTests
{
    using Core.Model;
    using Core.Repositories;
    using Core.Services;
    using Fanex.Data.Repository;
    using Microsoft.Extensions.Options;
    using Models;
    using NSubstitute;
    using Services;
    using Xunit;

    public class UserServiceTests
    {
        private readonly UserService userService;

        private readonly IOptionsSnapshot<AppSettingsOptions> configuration;
        private readonly IBaseMailService emailService;
        private readonly IBaseService baseService;
        private IDynamicRepository dynamicRepository;
        private IBaseRepository baseRepository;

        public UserServiceTests()
        {
            baseService = GetBaseService();
            emailService = Substitute.For<IBaseMailService>();

            var appSettingsOptions = new AppSettingsOptions
            {
                SessionTimeout = 60,
                EnableEncrypt = true,
                CertificateFilePath = "D=\\Test.pfx",
                PasswordToSignTheCertificateFile = "123456",
                BaseHostUrl = "http=//localhost=56977"
            };

            configuration = Substitute.For<IOptionsSnapshot<AppSettingsOptions>>();
            configuration.Value.Returns(appSettingsOptions);

            userService = new UserService(emailService, configuration, baseService);
        }

        private static IDynamicRepository FakeDynamicRepository()
        {
            var repository = Substitute.For<IDynamicRepository>();

            repository.Get<User>(Arg.Any<ICriteria>()).Returns(new User());

            return repository;
        }

        private static IBaseRepository FakeBaseRepository()
        {
            var repository = Substitute.For<IBaseRepository>();

            var commandResult = new CommandResult
            {
                Code = 0
            };

            repository.ExecuteNonQuery(Arg.Any<ICriteria>()).Returns(commandResult);

            return repository;
        }

        private BaseService GetBaseService()
        {
            dynamicRepository = FakeDynamicRepository();
            baseRepository = FakeBaseRepository();
            var service = new BaseService(dynamicRepository, baseRepository);

            return service;
        }

        [Theory]
        [InlineData("test")]
        public void GenerateTokenOTPRegister_Username_ReturnToken(string username)
        {
            var token = userService.GenerateTokenOTPRegister(username);

            Assert.NotEmpty(token);
        }

        [Theory]
        [InlineData("test")]
        public void GetUserByUsername_Username_ReturnUser(string username)
        {
            var user = userService.GetUserByUsername(username);

            Assert.IsType<User>(user);
        }

        [Fact]
        public void ResetTokenOTPRegister_ReturnTrue()
        {
            var result = userService.ResetTokenOTPRegister("test");

            Assert.True(result);
        }

        [Fact]
        public void ResetUserPassword_Void()
        {
            userService.ResetUserPassword("test");
        }

        [Fact]
        public void SendMailToRegisterOTP_Void()
        {
            userService.SendMailToRegisterOTP("test");
        }

        [Fact]
        public void UpdateOTPStatus_UserIdAndOtpStatus_ReturnTrue()
        {
            var result = userService.UpdateOTPStatus(1, true);

            Assert.True(result);
        }

        [Fact]
        public void UpdateSecretKey_UsernameAndSecretKey_ReturnTrue()
        {
            var result = userService.UpdateSecretKey("test", "test");

            Assert.True(result);
        }
    }
}