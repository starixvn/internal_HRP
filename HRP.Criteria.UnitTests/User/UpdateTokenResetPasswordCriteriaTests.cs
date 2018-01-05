namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class UpdateTokenResetPasswordCriteriaTests
    {
        [Fact]
        public virtual void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var expected = "User_UpdateTokenResetPassword";
            var criteria = new UpdateTokenResetPasswordCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void IsValid_UsernameIsInvalid_ReturnFalse(string username)
        {
            // Arrange
            var criteria = new UpdateTokenResetPasswordCriteria
            {
                Username = username
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_UsernameIsValid_ReturnTrue()
        {
            // Arrange
            var token = "token";
            var criteria = new UpdateTokenResetPasswordCriteria
            {
                Username = "admin",
                TokenResetPassword = token
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
            Assert.Equal(criteria.TokenResetPassword, token);
        }
    }
}