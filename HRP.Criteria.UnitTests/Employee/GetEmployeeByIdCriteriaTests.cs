namespace HRP.Criteria.UnitTests.Employee
{
    using HRP.Core.Constants;
    using HRP.Criteria.Employee;
    using Xunit;

    public class GetEmployeeByIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var expected = "Employee_GetByEmployeeId";
            var criteria = new GetEmployeeByIdCriteria();
            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(99)]
        public void IsValid_WithValidParams_ReturnTrue(int employeeId)
        {
            // Arrange
            var criteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = employeeId,
                FunctionKey = FunctionKey.EmployeeProfile,
                UserId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-11)]
        public void IsValid_WithInValidParams_ReturnFalse(int employeeId)
        {
            // Arrange
            var criteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = employeeId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}