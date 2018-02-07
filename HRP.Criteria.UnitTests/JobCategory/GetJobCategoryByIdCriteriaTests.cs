namespace HRP.Criteria.UnitTests.JobCategory
{
    using HRP.Criteria.JobCategory;
    using Xunit;

    public class GetJobCategoryByIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetJobCategoryByIdCriteria();
            var expected = "JobCategory_GetById";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_JobGradeIdIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetJobCategoryByIdCriteria
            {
                JobCategoryId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(0)]
        public void IsValid_JobGradeIdIsInvalid_ReturnFalse(int id)
        {
            // Arrange
            var criteria = new GetJobCategoryByIdCriteria
            {
                JobCategoryId = id
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}