namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class GetUserByTokenOTPRegisterCriteriaTests
    {
        [Fact]
        public virtual void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var expected = "User_GetByTokenOTPRegister";
            var criteria = new GetUserByTokenOTPRegisterCriteria();

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
            var criteria = new GetUserByTokenOTPRegisterCriteria
            {
                TokenOTPRegister = token
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
            var criteria = new GetUserByTokenOTPRegisterCriteria
            {
                TokenOTPRegister = "token"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}