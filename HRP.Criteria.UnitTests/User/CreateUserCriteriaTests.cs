namespace HRP.Criteria.UnitTests
{
    using System.Collections.Generic;
    using Xunit;

    public class CreateUserCriteriaTests
    {
        public static IEnumerable<object[]> TestCase()
        {
            yield return new object[] { "username", "password", string.Empty };
            yield return new object[] { "username", string.Empty, "myemail@email.com" };
            yield return new object[] { "username", string.Empty, string.Empty };
            yield return new object[] { string.Empty, "password", "myemail@email.com" };
            yield return new object[] { string.Empty, "password", string.Empty };
            yield return new object[] { string.Empty, string.Empty, "myemail@email.com" };
            yield return new object[] { string.Empty, string.Empty, string.Empty };
        }

        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CreateUserCriteria();
            var expected = "User_CreateUser";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory, MemberData(nameof(TestCase))]
        public void IsValid_ParamsAreInvalid_ReturnFalse(string username, string password, string email)
        {
            // Arrange
            var criteria = new CreateUserCriteria
            {
                Username = username,
                Password = password,
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
            var criteria = new CreateUserCriteria
            {
                Username = "username",
                Password = "password",
                Email = "myemail@email.com"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}