namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class GetJobGradeByGradeLevelIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetJobGradeByGradeLevelIdCriteria();
            var expected = "JobGrade_GetByGradeLevelId";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void IsValid_GradeLevelIdIsInvalid_ReturnFalse(int gradeLevelId)
        {
            // Arrange
            var criteria = new GetJobGradeByGradeLevelIdCriteria
            {
                GradeLevelId = gradeLevelId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_GradeLevelIdIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetJobGradeByGradeLevelIdCriteria
            {
                GradeLevelId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}