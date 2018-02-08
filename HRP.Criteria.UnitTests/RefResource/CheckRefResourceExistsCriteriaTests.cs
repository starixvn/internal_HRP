namespace HRP.Criteria.UnitTests.RefResource
{
    using HRP.Criteria.RefResource;
    using Xunit;

    public class CheckRefResourceExistsCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CheckRefResourceExistsCriteria();
            var expected = "RefResource_CheckExists";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, "ahihi")]
        [InlineData(1, "ahihi")]
        public void IsValid_ParamsAreValid_ReturnTrue(int id, string name)
        {
            // Arrange
            var criteria = new CheckRefResourceExistsCriteria
            {
                ResourceId = id,
                RefName = name,
                Value = name
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(-1, "   ")]
        [InlineData(-1, "")]
        [InlineData(-1, "asdasd")]
        [InlineData(1, "")]
        [InlineData(0, "")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int id, string name)
        {
            // Arrange
            var criteria = new CheckRefResourceExistsCriteria
            {
                ResourceId = id,
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