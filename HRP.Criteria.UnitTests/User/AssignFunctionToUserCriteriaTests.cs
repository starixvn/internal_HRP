namespace HRP.Criteria.UnitTests.User
{
    using System.Collections.Generic;
    using Xunit;

    public class AssignFunctionToUserCriteriaTests
    {
        public static IEnumerable<object[]> TestCase()
        {
            yield return new object[]
            {
                1, 1, "<root></root>", true
            };
            yield return new object[]
            {
                0, 1, "<root></root>", false
            };
            yield return new object[]
            {
                0, 0, "<root></root>", false
            };
            yield return new object[]
            {
                0, 0, string.Empty, false
            };
        }

        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new AssignFunctionToUserCriteria();
            var expected = "User_AssignFunction";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory, MemberData(nameof(TestCase))]
        public void IsValid_CheckParameter_ReturnAsExpected(int userId, int modifiedBy, string value, bool expected)
        {
            // Arrange
            var criteria = new AssignFunctionToUserCriteria
            {
                UserId = userId,
                ModifiedBy = modifiedBy,
                Value = value
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}