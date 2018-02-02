namespace HRP.Criteria.UnitTests.User
{
    using Xunit;

    public class GetUserByIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetUserByIdCriteria();
            var expected = "User_GetById";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void IsValid_UserIdIsInvalid_ReturnFalse(int userId)
        {
            // Arrange
            var criteria = new GetUserByIdCriteria
            {
                UserId = userId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_UserIdIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetUserByIdCriteria
            {
                UserId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}