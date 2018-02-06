namespace HRP.Criteria.UnitTests.JobGrade
{
    using HRP.Criteria.JobGrade;
    using Xunit;

    public class GetJobGradeByIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetJobGradeByIdCriteria();
            var expected = "JobGrade_GetById";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_JobGradeIdIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetJobGradeByIdCriteria
            {
                JobGradeId = 1
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
            var criteria = new GetJobGradeByIdCriteria
            {
                JobGradeId = id
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}