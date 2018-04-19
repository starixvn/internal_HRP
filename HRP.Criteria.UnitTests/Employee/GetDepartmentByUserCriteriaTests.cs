namespace HRP.Criteria.UnitTests.Employee
{
    using HRP.Criteria.Employee;
    using Xunit;

    public class GetDepartmentByUserCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetDepartmentByUserCriteria();
            var expected = "Employee_GetDepartment";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_UserIdIsLessThanZero_ReturnFalse()
        {
            // Arrange
            var criteria = new GetDepartmentByUserCriteria { UserId = -1 };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_UserIdIsGreaterThanZero_ReturnTrue()
        {
            // Arrange
            var criteria = new GetDepartmentByUserCriteria { UserId = 1 };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}