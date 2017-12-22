namespace HRP.Criteria.UnitTests.Function
{
    using System.Collections.Generic;
    using Xunit;

    public class GetFunctionByUserIdCriteriaTests
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
            var criteria = new GetFunctionByUserIdCriteria();
            var expected = "Function_GetByUserId";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory, MemberData(nameof(TestCase))]
        public void IsValid_CheckParams_ReturnAsExpected(int userId, bool expected)
        {
            // Arrange
            var criteria = new GetFunctionByUserIdCriteria
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