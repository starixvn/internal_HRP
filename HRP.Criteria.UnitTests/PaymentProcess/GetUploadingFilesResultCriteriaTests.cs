namespace HRP.Criteria.UnitTests.PaymentProcess
{
    using HRP.Criteria.PaymentProcess;
    using Xunit;

    public class GetUploadingFilesResultCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetUploadingFilesResultCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("PaymentProcess_GetUploadingFilesResult", actual);
        }

        [Theory]
        [InlineData(1)]
        public void IsValid_ParamsAreValid_ReturnTrue(int id)
        {
            // Arrange
            var criteria = new GetUploadingFilesResultCriteria
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
            var criteria = new GetUploadingFilesResultCriteria
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