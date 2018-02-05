namespace HRP.Criteria.UnitTests.Department
{
    using HRP.Criteria.Department;
    using Xunit;

    public class GetDepartmentByIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetDepartmentByIdCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("Department_GetById", actual);
        }

        [Theory]
        [InlineData(1)]
        public void IsValid_DepartmentIdIsValid_ReturnTrue(int id)
        {
            // Arrange
            var criteria = new GetDepartmentByIdCriteria
            {
                DepartmentId = id
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(0)]
        public void IsValid_DepartmentIdIsInvalid_ReturnFalse(int id)
        {
            // Arrange
            var criteria = new GetDepartmentByIdCriteria
            {
                DepartmentId = id
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}