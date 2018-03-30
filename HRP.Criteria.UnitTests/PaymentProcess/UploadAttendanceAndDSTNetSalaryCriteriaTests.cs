namespace HRP.Criteria.UnitTests.PaymentProcess
{
    using HRP.Criteria.PaymentProcess;
    using Xunit;

    public class UploadAttendanceAndDSTNetSalaryCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UploadAttendanceAndDSTNetSalaryCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("PaymentProcess_UploadAttendanceAndDSTNetSalary", actual);
        }

        [Theory]
        [InlineData(1)]
        public void IsValid_ParamsAreValid_ReturnTrue(int id)
        {
            // Arrange
            var criteria = new UploadAttendanceAndDSTNetSalaryCriteria
            {
                CompanyId = id,
                CreatedBy = id
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
            var criteria = new UploadAttendanceAndDSTNetSalaryCriteria
            {
                CompanyId = id,
                CreatedBy = id
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}