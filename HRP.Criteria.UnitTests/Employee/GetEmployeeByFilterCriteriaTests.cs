namespace HRP.Criteria.UnitTests.Employee
{
    using HRP.Criteria.Employee;
    using Xunit;

    public class GetEmployeeByFilterCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var expected = "Employee_GetAllEmployees";
            var criteria = new GetAllEmployeeCriteria();
            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WithValidParams_ReturnTrue()
        {
            // Arrange
            var criteria = new GetAllEmployeeCriteria
            {
                UserId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_WithInValidParams_ReturnFalse(int userId)
        {
            // Arrange
            var criteria = new GetAllEmployeeCriteria
            {
                EmployeeId = userId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}