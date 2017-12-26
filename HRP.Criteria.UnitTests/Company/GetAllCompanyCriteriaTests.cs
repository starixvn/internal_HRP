namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class GetAllCompanyCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetAllCompanyCriteria
            {
                IsActive = true
            };
            var expected = "Company_GetAll";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
            Assert.True(criteria.IsActive);
        }

        [Fact]
        public void IsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetAllCompanyCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}