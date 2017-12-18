namespace HRP.Criteria.UnitTests.User
{
    using System;
    using Xunit;

    public class UpdateSecretKeyCriteriaTests
    {
        private readonly UpdateSecretKeyCriteria criteria;
        private readonly string settingKey;

        public UpdateSecretKeyCriteriaTests()
        {
            criteria = new UpdateSecretKeyCriteria();
            settingKey = "User_UpdateSecretKey";
        }

        [Fact]
        public virtual void GetSettingKey_ReturnCorrectSettingKey()
        {
            var actual = criteria.GetSettingKey();
            Assert.Equal(settingKey, actual);
        }

        [Theory]
        [InlineData("", "", false)]
        [InlineData(null, null, false)]
        [InlineData(null, "", false)]
        [InlineData("", null, false)]
        [InlineData("test", "test", true)]
        public void IsValid_CheckParams_ReturnAsExpected(string username, string secretKey, bool expected)
        {
            // Arrange
            criteria.SecretKey = secretKey;
            criteria.Username = username;

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}