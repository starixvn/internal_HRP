namespace HRP.Criteria.UnitTests.Department
{
    using HRP.Criteria.Department;
    using Xunit;

    public class CheckDepartmentExistsCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CheckDepartmentExistsCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("Department_CheckExists", actual);
        }

        [Theory]
        [InlineData("ahihi")]
        public void IsValid_CodeIsValid_ReturnTrue(string code)
        {
            // Arrange
            var criteria = new CheckDepartmentExistsCriteria
            {
                Code = code
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData("   ")]
        [InlineData("")]
        public void IsValid_CodeIsInvalid_ReturnFalse(string code)
        {
            // Arrange
            var criteria = new CheckDepartmentExistsCriteria
            {
                Code = code
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}