namespace HRP.Criteria.UnitTests.RefResource
{
    using HRP.Criteria.RefResource;
    using Xunit;

    public class CreateRefResourceCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CreateRefResourceCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("RefResource_Create", actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new CreateRefResourceCriteria
            {
                CreatedBy = 2,
                RefName = "aa",
                Value = "ddd"
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
        public void IsValid_ParamsAreInvalid_ReturnFalse(int id, string name)
        {
            // Arrange
            var criteria = new CreateRefResourceCriteria
            {
                CreatedBy = id,
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