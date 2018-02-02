namespace HRP.Criteria.UnitTests.User
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
            var userId = 1;
            var phone = string.Empty;
            var isActive = true;
            var editedBy = 1;
            var fullName = string.Empty;
            var email = "myemail@email.com";
            var errCode = 0;
            var errMsg = string.Empty;

            var criteria = new EditUserCriteria
            {
                UserId = userId,
                FullName = fullName,
                Phone = phone,
                IsActive = isActive,
                EditedBy = editedBy,
                Email = email,
                ErrCode = errCode,
                ErrMsg = errMsg
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
            Assert.Equal(criteria.UserId, userId);
            Assert.Equal(criteria.FullName, fullName);
            Assert.Equal(criteria.Phone, phone);
            Assert.Equal(criteria.Email, email);
            Assert.Equal(criteria.IsActive, isActive);
            Assert.Equal(criteria.EditedBy, editedBy);
            Assert.Equal(criteria.ErrCode, errCode);
            Assert.Equal(criteria.ErrMsg, errMsg);
        }
    }
}