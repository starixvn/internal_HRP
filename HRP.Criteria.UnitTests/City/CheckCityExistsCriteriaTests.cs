namespace HRP.Criteria.UnitTests.City
{
    using HRP.Criteria.City;
    using Xunit;

    public class CheckCityExistsCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CheckCityExistsCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("City_CheckExists", actual);
        }

        [Theory]
        [InlineData("ahihi")]
        public void IsValid_CodeIsValid_ReturnTrue(string code)
        {
            // Arrange
            var criteria = new CheckCityExistsCriteria
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
            var criteria = new CheckCityExistsCriteria
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