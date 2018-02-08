namespace HRP.Criteria.UnitTests.Country
{
    using HRP.Criteria.Country;
    using Xunit;

    public class UpdateCountryCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UpdateCountryCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("Country_Update", actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new UpdateCountryCriteria
            {
                CountryId = 1,
                UpdatedBy = 2,
                Code = "aa"
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
        public void IsValid_ParamsAreInvalid_ReturnFalse(int id, string code)
        {
            // Arrange
            var criteria = new UpdateCountryCriteria
            {
                CountryId = id,
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