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
            Assert.IsAssignableFrom<BaseNonQueryCommand>(criteria);
        }

        [Theory]
        [InlineData(-1, "   ")]
        [InlineData(-1, "")]
        [InlineData(-1, "asdasd")]
        [InlineData(0, "asdasd")]
        [InlineData(1, " ")]
        [InlineData(0, " ")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int id, string code)
        {
            // Arrange
            var criteria = new UpdateCostCenterCriteria
            {
                CostCenterId = id,
                UpdatedBy = id,
                Code = code
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}