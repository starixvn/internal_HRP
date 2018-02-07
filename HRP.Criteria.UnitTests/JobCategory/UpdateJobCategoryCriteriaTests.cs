namespace HRP.Criteria.UnitTests.JobCategory
{
    using HRP.Criteria.JobCategory;
    using Xunit;

    public class UpdateJobCategoryCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UpdateJobCategoryCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("JobCategory_Update", actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new UpdateJobCategoryCriteria
            {
                JobCategoryId = 1,
                UpdatedBy = 2,
                Name = "aa"
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
            var criteria = new UpdateJobCategoryCriteria
            {
                JobCategoryId = id,
                UpdatedBy = id,
                Name = name
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}