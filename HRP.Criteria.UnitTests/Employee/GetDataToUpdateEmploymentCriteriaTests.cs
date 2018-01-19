namespace HRP.Criteria.UnitTests
{
    using HRP.Criteria.Employee;
    using Xunit;

    public class GetDataToUpdateEmploymentCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetDataToUpdateEmploymentCriteria();
            var expected = "Employment_GetDataToUpdate";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(0, 1)]
        [InlineData(0, 0)]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int userId, int employeeId)
        {
            // Arrange
            var criteria = new GetDataToUpdateEmploymentCriteria { UserId = userId, EmployeeId = employeeId };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetDataToUpdateEmploymentCriteria { UserId = 1, EmployeeId = 1 };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}