namespace HRP.Criteria.UnitTests.CostCenter
{
    using HRP.Criteria.CostCenter;
    using Xunit;

    public class CreateCostCenterCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CreateCostCenterCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("CostCenter_Create", actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new CreateCostCenterCriteria
            {
                DepartmentId = 1,
                CreatedBy = 2,
                Code = "aa"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData("   ")]
        [InlineData("")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(string code)
        {
            // Arrange
            var criteria = new CreateCostCenterCriteria
            {
                DepartmentId = 0,
                CreatedBy = 0,
                Code = code
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}