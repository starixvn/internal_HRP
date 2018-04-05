namespace HRP.Criteria.UnitTests
{
    using HRP.Criteria.Payslip;
    using Xunit;

    public class UpdatePayslipCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UpdatePayslipCriteria();
            var expected = "Payslip_Update";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 0, "1")]
        [InlineData(1, 0, "")]
        [InlineData(0, 1, "1")]
        [InlineData(0, 1, "")]
        [InlineData(0, 0, "1")]
        [InlineData(0, 0, "")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int companyId, int userId, string payrollIds)
        {
            // Arrange
            var criteria = new UpdatePayslipCriteria
            {
                CompanyId = companyId,
                UserId = userId,
                PayrollIds = payrollIds
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
            var criteria = new UpdatePayslipCriteria
            {
                CompanyId = 1,
                UserId = 1,
                PayrollIds = "1,2"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}