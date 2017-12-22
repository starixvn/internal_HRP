namespace HRP.Criteria.UnitTests
{
    using System.Collections.Generic;
    using Xunit;

    public class GetMenuByUserIdCriteriaTests
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
            var criteria = new GetMenuByUserIdCriteria();
            var expected = "Menu_GetByUserId";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory, MemberData(nameof(TestCase))]
        public void IsValid_CheckParams_ReturnAsExpected(int userId, bool expected)
        {
            // Arrange
            var criteria = new GetMenuByUserIdCriteria
            {
                UserId = userId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}