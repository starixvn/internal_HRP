namespace HRP.Criteria.UnitTests.Office
{
    using HRP.Criteria.Office;
    using Xunit;

    public class GetAllOfficeCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetAllOfficeCriteria();
            var expected = "Office_GetAll";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetAllOfficeCriteria
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