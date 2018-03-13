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

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new EditMonthlySalaryCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}