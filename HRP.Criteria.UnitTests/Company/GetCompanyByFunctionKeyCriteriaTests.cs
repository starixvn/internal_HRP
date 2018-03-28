namespace HRP.Criteria.UnitTests.Company
{
    using Xunit;

    public class GetCompanyByFunctionKeyCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetCompanyByFunctionKeyCriteria();
            var expected = "Company_GetByFunctionKey";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, "")]
        [InlineData(0, "Payslip")]
        [InlineData(0, "")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int userId, string functionKey)
        {
            // Arrange
            var criteria = new GetCompanyByFunctionKeyCriteria
            {
                UserId = userId,
                FunctionKey = functionKey
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
            var criteria = new GetCompanyByFunctionKeyCriteria
            {
                UserId = 1,
                FunctionKey = "Payslip"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}