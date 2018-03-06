namespace HRP.Criteria.UnitTests
{
    using HRP.Criteria.MonthlyPayroll;
    using Xunit;

    public class UpdateMonthlySalaryCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UpdateMonthlySalaryCriteria();
            var expected = "MonthlyPayroll_UpdateSalary";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}