namespace HRP.Criteria.UnitTests.MonthlyPayroll
{
    using HRP.Criteria.MonthlyPayroll;
    using Xunit;

    public class GetDataFilterForIndexPageCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var expected = "MonthlyPayroll_GetDataFilterForIndexPage";
            var criteria = new GetDataFilterForIndexPageCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetDataFilterForIndexPageCriteria
            {
                UserId = 1,
                CompanyId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(0, 1)]
        [InlineData(0, 0)]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int userId, int companyId)
        {
            // Arrange
            var criteria = new GetDataFilterForIndexPageCriteria
            {
                UserId = userId,
                CompanyId = companyId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}