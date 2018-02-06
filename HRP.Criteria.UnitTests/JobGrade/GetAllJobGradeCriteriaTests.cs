namespace HRP.Criteria.UnitTests.JobGrade
{
    using HRP.Criteria.JobGrade;
    using Xunit;

    public class GetAllJobGradeCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetAllJobGradeCriteria();
            var expected = "JobGrade_GetAll";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetAllJobGradeCriteria
            {
                IsActive = true
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}