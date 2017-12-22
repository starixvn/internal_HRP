namespace HRP.Core.UnitTests
{
    using System.Collections.Generic;
    using HRP.Core.Criteria;
    using Xunit;

    public class LogOutUserCriteriaTests
    {
        public static IEnumerable<object[]> TestCase()
        {
            yield return new object[]
            {
                1, true
            };
            yield return new object[]
            {
                0, false
            };
        }

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

        [Theory, MemberData(nameof(TestCase))]
        public void IsValid_CheckParams_ReturnAsExpected(int userId, bool expected)
        {
            // Arrange
            var criteria = new LogOutUserCriteria
            {
                OnUserId = userId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}