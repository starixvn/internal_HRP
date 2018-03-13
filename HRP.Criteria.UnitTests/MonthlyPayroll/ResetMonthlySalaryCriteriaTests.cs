namespace HRP.Criteria.UnitTests
{
    using HRP.Criteria.MonthlyPayroll;
    using Xunit;

    public class ResetMonthlySalaryCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new ResetMonthlySalaryCriteria();
            var expected = "MonthlyPayroll_ResetSalary";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new ResetMonthlySalaryCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}