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
        [InlineData("")]
        [InlineData(null)]
        public void IsValid_TokenIsInvalid_ReturnFalse(string token)
        {
            // Arrange
            var criteria = new GetUserByTokenResetPasswordCriteria
            {
                TokenResetPassword = token
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_TokenIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetUserByTokenResetPasswordCriteria
            {
                TokenResetPassword = "token"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}