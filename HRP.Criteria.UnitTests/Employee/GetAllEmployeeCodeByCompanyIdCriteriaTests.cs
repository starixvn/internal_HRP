namespace HRP.Criteria.UnitTests.Employee
{
    using HRP.Criteria.Employee;
    using Xunit;

    public class GetAllEmployeeCodeByCompanyIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetAllEmployeeCodeByCompanyIdCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("Employment_GetAllEmployeeCodeByCompanyId", actual);
        }

        [Theory]
        [InlineData(1)]
        public void IsValid_ParamsAreValid_ReturnTrue(int id)
        {
            // Arrange
            var criteria = new GetAllEmployeeCodeByCompanyIdCriteria
            {
                CompanyId = id,
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int id)
        {
            // Arrange
            var criteria = new GetAllEmployeeCodeByCompanyIdCriteria
            {
                CompanyId = id,
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}