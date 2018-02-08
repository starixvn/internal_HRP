namespace HRP.Criteria.UnitTests.RefResource
{
    using System;
    using HRP.Criteria.RefResource;
    using Xunit;

    public class UpdateRefResourceItemCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UpdateRefResourceItemCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("RefResource_Update", actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new UpdateRefResourceItemCriteria
            {
                ResourceId = 1,
                UpdatedBy = 2,
                RefName = "aa",
                Value = "vvv"
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
        public void IsValid_ParamsAreInvalid_ReturnFalse(int id, string name)
        {
            // Arrange
            var criteria = new UpdateRefResourceItemCriteria
            {
                ResourceId = id,
                UpdatedBy = id,
                RefName = name,
                Value = name
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}