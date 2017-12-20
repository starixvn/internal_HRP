namespace HRP.Criteria.UnitTests.User
{
    using Xunit;

    public class UpdateTokenResetPasswordCriteriaTests
    {
        private readonly UpdateTokenResetPasswordCriteria criteria;
        private readonly string settingKey;

        public UpdateTokenResetPasswordCriteriaTests()
        {
            criteria = new UpdateTokenResetPasswordCriteria();
            settingKey = "User_UpdateTokenResetPassword";
        }

        [Fact]
        public virtual void GetSettingKey_ReturnCorrectSettingKey()
        {
            var actual = criteria.GetSettingKey();
            Assert.Equal(settingKey, actual);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("test", true)]
        public void IsValid_CheckParams_ReturnAsExpected(string username, bool expected)
        {
            // Arrange
            criteria.Username = username;

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}