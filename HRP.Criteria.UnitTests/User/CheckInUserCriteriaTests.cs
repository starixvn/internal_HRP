namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class CheckInUserCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CheckInUserCriteria();
            var expected = "User_CheckIn";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, null)]
        [InlineData(0, "session")]
        [InlineData(0, null)]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int userId, string sessionId)
        {
            // Arrange
            var ip = string.Empty;
            var onUserId = 1;
            var platform = string.Empty;
            var criteria = new CheckInUserCriteria
            {
                UserId = userId,
                NetSessionId = sessionId,
                IP = ip,
                OnUserId = onUserId,
                Platform = platform
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
            Assert.Equal(criteria.IP, ip);
            Assert.Equal(criteria.OnUserId, onUserId);
            Assert.Equal(criteria.Platform, platform);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new CheckInUserCriteria
            {
                UserId = 1,
                NetSessionId = "sessionId"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}