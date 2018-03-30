namespace HRP.Criteria.UnitTests.PaymentProcess
{
    using HRP.Criteria.PaymentProcess;
    using Xunit;

    public class GenerateMonthlyPayrollCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GenerateMonthlyPayrollCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("PaymentProcess_GenerateMonthlyPayroll", actual);
        }

        [Theory]
        [InlineData(1, "ahihi")]
        public void IsValid_ParamsAreValid_ReturnTrue(int id, string text)
        {
            // Arrange
            var criteria = new GenerateMonthlyPayrollCriteria
            {
                CompanyId = id,
                UpdatedBy = id,
                PayrollXMLDoc = text,
                BasicSalaryXMLDoc = text,
                SalaryXMLDoc = text,
                BasicAllowanceXMLDoc = text,
                AllowanceXMLDoc = text
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
            Assert.IsAssignableFrom<CommandBaseCriteria>(criteria);
        }

        [Theory]
        [InlineData(-1, "   ")]
        [InlineData(-1, "")]
        [InlineData(-1, "asdasd")]
        [InlineData(0, "asdasd")]
        [InlineData(1, " ")]
        [InlineData(0, " ")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int id, string text)
        {
            // Arrange
            var criteria = new GenerateMonthlyPayrollCriteria
            {
                CompanyId = id,
                UpdatedBy = id,
                PayrollXMLDoc = text,
                BasicSalaryXMLDoc = text,
                SalaryXMLDoc = text,
                BasicAllowanceXMLDoc = text,
                AllowanceXMLDoc = text
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}