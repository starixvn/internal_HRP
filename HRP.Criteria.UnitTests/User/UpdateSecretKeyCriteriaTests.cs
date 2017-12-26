namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class UpdateSecretKeyCriteriaTests
    {
        [Fact]
        public virtual void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var expected = "User_UpdateSecretKey";
            var criteria = new UpdateSecretKeyCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("secret-key", "")]
        [InlineData("", "admin")]
        [InlineData("", "")]
        public void IsValid_CheckParams_ReturnAsExpected(string username, string secretKey)
        {
            // Arrange
            var criteria = new UpdateSecretKeyCriteria
            {
                SecretKey = secretKey,
                Username = username
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new UpdateSecretKeyCriteria
            {
                SecretKey = "secret-key",
                Username = "admin"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}