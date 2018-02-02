namespace HRP.Criteria.UnitTests.GradeLevel
{
    using Xunit;

    public class GetGradeLevelByJobCategoryIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetGradeLevelByJobCategoryIdCriteria();
            var expected = "GradeLevel_GetByJobCategoryId";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void IsValid_JobCategoryIdIsInvalid_ReturnFalse(int jobCategoryId)
        {
            // Arrange
            var criteria = new GetGradeLevelByJobCategoryIdCriteria
            {
                JobCategoryId = jobCategoryId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_JobCategoryIdIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetGradeLevelByJobCategoryIdCriteria
            {
                JobCategoryId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}