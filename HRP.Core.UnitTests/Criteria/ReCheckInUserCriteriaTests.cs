namespace HRP.Core.UnitTests
{
    using HRP.Core.Criteria;
    using Xunit;

    public class ReCheckInUserCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new ReCheckInUserCriteria();
            var expected = "User_ReCheckIn";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void IsValid_OnUserIdIsInvalid_ReturnFalse(int userId)
        {
            // Arrange
            var criteria = new ReCheckInUserCriteria
            {
                OnUserId = userId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_OnUserIdIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new ReCheckInUserCriteria
            {
                OnUserId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}