namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class UpdateTokenOTPRegisterCriteriaTests
    {
        [Fact]
        public virtual void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var expected = "User_UpdateTokenOTPRegister";
            var criteria = new UpdateTokenOTPRegisterCriteria();

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
            var criteria = new UpdateTokenOTPRegisterCriteria
            {
                TokenOTPRegister = "token",
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
            var criteria = new UpdateTokenOTPRegisterCriteria {
                Username = "admin",
                TokenOTPRegister = token
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
            Assert.Equal(criteria.TokenOTPRegister, token);
        }
    }
}