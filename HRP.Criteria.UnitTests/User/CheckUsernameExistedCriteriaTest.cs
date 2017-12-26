namespace HRP.Criteria.UnitTests
{
    using HRP.Criteria;
    using Xunit;

    public class CheckUsernameExistedCriteriaTest
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CheckUsernameExistedCriteria();
            var expected = "User_CheckUsernameExisted";

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
            var criteria = new CheckUsernameExistedCriteria { Username = username };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_UsernameIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new CheckUsernameExistedCriteria { Username = "admin" };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}