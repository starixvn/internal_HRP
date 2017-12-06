namespace HRP.Tests.Core.Services
{
    using HRP.Core.Model;
    using HRP.Core.Services;
    using Microsoft.Extensions.Options;
    using Xunit;

    public class BaseMailServiceTests
    {
        readonly IOptionsSnapshot<MailConfig> _configuration;
        readonly BaseMailService _baseMailService;

        public BaseMailServiceTests()
        {
            //TODO: Init mail config here for testing
            _baseMailService = new BaseMailService(_configuration);
        }
    }
}
