namespace HRP.Criteria.UnitTests.GradeLevel
{
    using HRP.Criteria.GradeLevel;
    using Xunit;

    public class GetGradeLevelByIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetGradeLevelByIdCriteria();
            var expected = "GradeLevel_GetById";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_GradeLevelIdIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetGradeLevelByIdCriteria
            {
                GradeLevelId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(0)]
        public void IsValid_GradeLevelIdIsInvalid_ReturnFalse(int id)
        {
            // Arrange
            var criteria = new GetGradeLevelByIdCriteria
            {
                GradeLevelId = id
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}