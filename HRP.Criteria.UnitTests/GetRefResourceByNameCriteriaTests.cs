namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class GetRefResourceByNameCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetRefResourceByNameCriteria();
            var expected = "RefResource_GetByName";

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
            var criteria = new GetRefResourceByNameCriteria
            {
                RefName = refName
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
            var criteria = new GetRefResourceByNameCriteria { RefName = "office" };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}