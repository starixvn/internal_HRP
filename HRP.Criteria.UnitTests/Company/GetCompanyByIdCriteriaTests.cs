namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class GetCompanyByIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetCompanyByIdCriteria();
            var expected = "Company_GetById";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void IsValid_CompanyIdIsInvalid_ReturnFalse(int companyId)
        {
            // Arrange
            var criteria = new GetCompanyByIdCriteria
            {
                CompanyId = companyId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_CompanyIdIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetCompanyByIdCriteria
            {
                CompanyId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}