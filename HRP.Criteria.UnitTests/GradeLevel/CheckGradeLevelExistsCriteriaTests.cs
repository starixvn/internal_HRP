namespace HRP.Criteria.UnitTests.GradeLevel
{
    using HRP.Criteria.GradeLevel;
    using Xunit;

    public class CheckGradeLevelExistsCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CheckGradeLevelExistsCriteria();
            var expected = "GradeLevel_CheckExists";

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
            var criteria = new CheckGradeLevelExistsCriteria
            {
                GradeLevelId = id,
                JobCategoryId = id,
                Name = name
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
            var criteria = new CheckGradeLevelExistsCriteria
            {
                GradeLevelId = id,
                JobCategoryId = id,
                Name = name
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}