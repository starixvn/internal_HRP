namespace HRP.Criteria.UnitTests.CostCenter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using HRP.Criteria.CostCenter;
    using Xunit;

    public class UpdateCostCenterCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UpdateCostCenterCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("CostCenter_Update", actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new UpdateCostCenterCriteria
            {
                CostCenterId = 1,
                UpdatedBy = 2,
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
            var criteria = new UpdateCostCenterCriteria
            {
                CostCenterId = 0,
                UpdatedBy = 0,
                Code = code
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}