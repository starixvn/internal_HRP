namespace HRP.Criteria.UnitTests.CostCenter
{
    using HRP.Criteria.CostCenter;
    using Xunit;

    public class CheckCostCenterExistsCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CheckCostCenterExistsCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("CostCenter_CheckExists", actual);
        }

        [Theory]
        [InlineData("ahihi")]
        public void IsValid_CodeIsValid_ReturnTrue(string code)
        {
            // Arrange
            var criteria = new CheckCostCenterExistsCriteria
            {
                Code = code
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData("   ")]
        [InlineData("")]
        public void IsValid_CodeIsInvalid_ReturnFalse(string code)
        {
            // Arrange
            var criteria = new CheckCostCenterExistsCriteria
            {
                Code = code
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}