namespace HRP.Criteria.UnitTests
{
    using System.Collections.Generic;
    using Xunit;

    public class CheckInUserCriteriaTests
    {
        public static IEnumerable<object[]> TestCase()
        {
            yield return new object[]
            {
                1, "session", true
            };
            yield return new object[]
            {
                1, string.Empty, false
            };
            yield return new object[]
            {
                0, "session", false
            };
            yield return new object[]
            {
                0, string.Empty, false
            };
        }

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

        [Theory, MemberData(nameof(TestCase))]
        public void IsValid_CheckParams_ReturnAsExpected(int userId, string sessionId, bool expected)
        {
            // Arrange
            var criteria = new CheckInUserCriteria
            {
                UserId = userId,
                NetSessionId = sessionId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}