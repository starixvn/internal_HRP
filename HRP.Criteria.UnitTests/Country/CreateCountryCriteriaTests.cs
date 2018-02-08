namespace HRP.Criteria.UnitTests.Country
{
    using HRP.Criteria.Country;
    using Xunit;

    public class CreateCountryCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CreateCountryCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("Country_Create", actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new CreateCountryCriteria
            {
                CreatedBy = 2,
                Code = "wqew"
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
            var criteria = new CreateCountryCriteria
            {
                CreatedBy = id,
                Name = name
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}