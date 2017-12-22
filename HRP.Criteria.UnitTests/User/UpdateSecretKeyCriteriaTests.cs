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
        [InlineData("", "", false)]
        [InlineData(null, null, false)]
        [InlineData(null, "", false)]
        [InlineData("", null, false)]
        [InlineData("test", "test", true)]
        public void IsValid_CheckParams_ReturnAsExpected(string username, string secretKey, bool expected)
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
            Assert.Equal(expected, actual);
        }
    }
}