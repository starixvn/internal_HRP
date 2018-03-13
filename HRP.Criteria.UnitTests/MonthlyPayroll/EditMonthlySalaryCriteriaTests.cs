namespace HRP.Criteria.UnitTests
{
    using HRP.Criteria.MonthlyPayroll;
    using Xunit;

    public class EditMonthlySalaryCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new EditMonthlySalaryCriteria();
            var expected = "MonthlyPayroll_UpdateSalary";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 1, 0)]
        [InlineData(1, 0, 1)]
        [InlineData(1, 0, 0)]
        [InlineData(0, 1, 1)]
        [InlineData(0, 1, 0)]
        [InlineData(0, 0, 1)]
        [InlineData(0, 0, 0)]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int payrollId, int companyId, int userId)
        {
            // Arrange
            var criteria = new EditMonthlySalaryCriteria
            {
                PayrollId = payrollId,
                CompanyId = companyId,
                UserId = userId
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
            var criteria = new EditMonthlySalaryCriteria
            {
                PayrollId = 1,
                CompanyId = 1,
                UserId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}