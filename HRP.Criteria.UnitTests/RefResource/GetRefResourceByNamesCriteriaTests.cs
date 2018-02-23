namespace HRP.Criteria.UnitTests.RefResource
{
    using Criteria.RefResource;
    using Xunit;

    public class GetRefResourceByNamesCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetRefResourceByNamesCriteria();
            var expected = "RefResource_GetByNames";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void IsValid_RefNameIsInvalid_ReturnFalse(string refName)
        {
            // Arrange
            var criteria = new GetRefResourceByNamesCriteria
            {
                RefNames = refName
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_RefNameIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetRefResourceByNamesCriteria { RefNames = "office" };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}