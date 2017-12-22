namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class GetAllFunctionCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetAllFunctionCriteria();
            var expected = "Function_GetAll";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetAllFunctionCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}