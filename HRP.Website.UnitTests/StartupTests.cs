namespace HRP.Website.UnitTests
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class StartupTests
    {
        [Fact]
        public void ConfigureServices_RegistersDependenciesCorrectly()
        {
            //  Arrange
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            IServiceCollection services = new ServiceCollection();
            var target = new Startup(config);

            //  Act
            var ex = Record.Exception(() => target.ConfigureServices(services));

            //  Assert
            Assert.Null(ex);
        }
    }
}

