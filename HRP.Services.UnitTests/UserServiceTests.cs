namespace HRP.Services.UnitTests
{
    using Core.Model;
    using Core.Services;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using Services;

    public class UserServiceTests
    {
        private readonly UserService _userService;

        private readonly IOptionsSnapshot<AppSettingsOptions> _configuration;
        private readonly IBaseMailService _emailService;
        private readonly IBaseService _baseService;

        public UserServiceTests()
        {
            _baseService = Substitute.For<IBaseService>();
            _emailService = Substitute.For<IBaseMailService>();

            var appSettingsOptions = new AppSettingsOptions
            {
                SessionTimeout = 60,
                EnableEncrypt = true,
                CertificateFilePath = "D=\\Test.pfx",
                PasswordToSignTheCertificateFile = "123456",
                BaseHostUrl = "http=//localhost=56977"
            };

            _configuration = Substitute.For<IOptionsSnapshot<AppSettingsOptions>>();
            _configuration.Value.Returns(appSettingsOptions);

            _userService = new UserService(_emailService, _configuration, _baseService);
        }
    }
}