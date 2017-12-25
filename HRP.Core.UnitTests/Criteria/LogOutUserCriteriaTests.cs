namespace HRP.Core.UnitTests
{
    using HRP.Core.Criteria;
    using Xunit;

    public class LogOutUserCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new LogOutUserCriteria();
            var expected = "User_Logout";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void IsValid_UserIdLessThanZero_ReturnFalse(int userId)
        {
            // Arrange
            var criteria = new LogOutUserCriteria
            {
                OnUserId = userId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_UserIdGreaterThanZero_ReturnFalse()
        {
            // Arrange
            var criteria = new LogOutUserCriteria
            {
                OnUserId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}