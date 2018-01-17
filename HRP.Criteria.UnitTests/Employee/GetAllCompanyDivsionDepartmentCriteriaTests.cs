namespace HRP.Criteria.UnitTests.Employee
{
    using HRP.Criteria.Employee;
    using Xunit;

    public class GetAllCompanyDivsionDepartmentCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var expected = "Employee_GetAllCompanyDivisionDepartment";
            var criteria = new GetAllCompanyDivsionDepartmentCriteria();
            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1)]
        public void IsValid_WithValidParams_ReturnTrue(int userId)
        {
            // Arrange
            var criteria = new GetAllCompanyDivsionDepartmentCriteria
            {
                UserId = userId
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
            var criteria = new GetAllCompanyDivsionDepartmentCriteria
            {
                UserId = userId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}