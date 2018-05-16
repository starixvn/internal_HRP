namespace HRP.Criteria.UnitTests.Report
{
    using HRP.Criteria.Report;
    using Xunit;

    public class GetDataForEbankingReportCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetDataForEbankingReportCriteria();
            var expected = "Report_GetDataForEbankingReport";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(0, 1)]
        [InlineData(0, 0)]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int userId, int companyId)
        {
            // Arrange
            var criteria = new GetDataForEbankingReportCriteria
            {
                UserId = userId,
                CompanyId = companyId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetDataForEbankingReportCriteria
            {
                UserId = 1,
                CompanyId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}