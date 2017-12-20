namespace HRP.Criteria.UnitTests.User
{
    using Xunit;

    public class GetUserByTokenResetPasswordCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetUserByTokenResetPasswordCriteria();
            var expected = "User_GetByTokenResetPassword";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("abc", true)]
        public void IsValid_ReturnAsExpected(string token, bool expected)
        {
            var criteria = new GetUserByTokenResetPasswordCriteria
            {
                TokenResetPassword = token
            };

            Assert.Equal(expected, criteria.IsValid());
        }
    }
}