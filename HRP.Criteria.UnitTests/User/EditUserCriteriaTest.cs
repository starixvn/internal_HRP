namespace HRP.Criteria.UnitTests
{
    using System.Collections.Generic;
    using Xunit;

    public class EditUserCriteriaTest
    {
        public static IEnumerable<object[]> TestCase()
        {
            yield return new object[] { 1, 1, string.Empty };
            yield return new object[] { 1, 0, "myemail@email.com" };
            yield return new object[] { 1, 0, string.Empty };
            yield return new object[] { 0, 1, "myemail@email.com" };
            yield return new object[] { 0, 1, string.Empty };
            yield return new object[] { 0, 0, "myemail@email.com" };
            yield return new object[] { 0, 0, string.Empty };
        }

        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new EditUserCriteria();
            var expected = "User_EditUser";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory, MemberData(nameof(TestCase))]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int userId, int editedBy, string email)
        {
            // Arrange
            var criteria = new EditUserCriteria
            {
                UserId = userId,
                EditedBy = editedBy,
                Email = email
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
            var criteria = new EditUserCriteria
            {
                UserId = 1,
                EditedBy = 1,
                Email = "myemail@email.com"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}