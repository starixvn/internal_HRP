namespace HRP.Criteria.UnitTests
{
    using HRP.Criteria.MonthlyPayroll;
    using Xunit;

    public class GetMonthlySalaryToUpdateCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetMonthlySalaryToEditCriteria();
            var expected = "MonthlyPayroll_GetSalary";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(0, 1)]
        [InlineData(0, 0)]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int userId, int payrollId)
        {
            // Arrange
            var criteria = new GetMonthlySalaryToEditCriteria
            {
                UserId = userId,
                PayrollId = payrollId
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
            var criteria = new GetMonthlySalaryToEditCriteria
            {
                UserId = 1,
                PayrollId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}