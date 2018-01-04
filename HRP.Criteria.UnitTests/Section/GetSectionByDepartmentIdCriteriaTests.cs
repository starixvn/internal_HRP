namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class GetSectionByDepartmentIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetSectionByDepartmentIdCriteria();
            var expected = "Section_GetByDepartmentId";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void IsValid_DepartmentIdIsInvalid_ReturnFalse(int departmentId)
        {
            // Arrange
            var criteria = new GetSectionByDepartmentIdCriteria
            {
                DepartmentId = departmentId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_DepartmentIdIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetSectionByDepartmentIdCriteria
            {
                DepartmentId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}