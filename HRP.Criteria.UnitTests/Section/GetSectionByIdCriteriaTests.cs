namespace HRP.Criteria.UnitTests.Section
{
    using HRP.Criteria.Section;
    using Xunit;

    public class GetSectionByIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetSectionByIdCriteria();
            var expected = "Section_GetById";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_SectionIdIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetSectionByIdCriteria
            {
                SectionId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(0)]
        public void IsValid_SectionIdIsInvalid_ReturnFalse(int id)
        {
            // Arrange
            var criteria = new GetSectionByIdCriteria
            {
                SectionId = id
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}