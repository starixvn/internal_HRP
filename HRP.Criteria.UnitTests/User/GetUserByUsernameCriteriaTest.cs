namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class GetUserByUsernameCriteriaTest
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetUserByUsernameCriteria();
            var expected = "User_GetByUsername";

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
            var criteria = new GetUserByUsernameCriteria
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
            var criteria = new GetUserByUsernameCriteria { Username = "admin" };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}