namespace HRP.Criteria.UnitTests.Employee
{
    using HRP.Criteria.Employee;
    using Xunit;

    public class GetEmployeeStatusByUserCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetEmployeeStatusByUserCriteria();
            var expected = "Employee_GetEmployeeStatus";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_UserIdIsLessThanZero_ReturnFalse()
        {
            // Arrange
            var criteria = new GetEmployeeStatusByUserCriteria { UserId = -1 };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_UserIdIsGreaterThanZero_ReturnTrue()
        {
            // Arrange
            var criteria = new GetEmployeeStatusByUserCriteria { UserId = 1 };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}