namespace HRP.Criteria.UnitTests.User
{
    using System.Collections.Generic;
    using Xunit;

    public class AssignFunctionToUserCriteriaTests
    {
        public static IEnumerable<object[]> TestCase()
        {
            yield return new object[] { 1, 1, string.Empty };
            yield return new object[] { 1, 0, "<root></root>" };
            yield return new object[] { 1, 0, string.Empty };
            yield return new object[] { 0, 1, "<root></root>" };
            yield return new object[] { 0, 1, string.Empty };
            yield return new object[] { 0, 0, "<root></root>" };
            yield return new object[] { 0, 0, string.Empty };
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
        public void IsValid_ParamsAreInvalid_ReturnFalse(int userId, int modifiedBy, string value)
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
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new AssignFunctionToUserCriteria
            {
                UserId = 1,
                ModifiedBy = 1,
                Value = "<root></root>"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}