namespace HRP.Core.UnitTests.Services
{
    using System;
    using System.IO;
    using HRP.Core.Services;
    using NSubstitute;
    using Xunit;

    public class BaseMailServiceTests
    {
        private BaseMailService baseMailService;

        [Fact]
        public void ReadTextFile_InvalidPathFile_ThrowArgumentException()
        {
            // Arrange
            FakeBaseMailService();

            // Act && Assert
            Assert.Throws<ArgumentException>((Action)(() => baseMailService.ReadTextFile(string.Empty)));
        }

        [Fact]
        public void ReadTextFile_InvalidPathFile_ThrowFileNotFoundException()
        {
            // Arrange
            FakeBaseMailService();

            // Act && Assert
            Assert.Throws<FileNotFoundException>((Action)(() => baseMailService.ReadTextFile("aaa.txt")));
        }

        [Fact]
        public void ReadTextFile_ValidPathFile_ReturnAsExpected()
        {
            // Arrange
            FakeBaseMailService();

            // Act
            var actual = baseMailService.ReadTextFile("App_Data\\Test.txt");

            // Assert
            Assert.Equal("Hello", actual);
        }

        private void FakeBaseMailService()
        {
            var baseService = Substitute.For<IBaseService>();

            baseMailService = new BaseMailService(baseService);
        }
    }
}