namespace HRP.Criteria.UnitTests.Division
{
    using Xunit;

    public class GetDivisionByIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetDivisionByIdCriteria();
            var expected = "Division_GetById";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void IsValid_DivisionIdIsInvalid_ReturnFalse(int divisionId)
        {
            // Arrange
            var criteria = new GetDivisionByIdCriteria
            {
                DivisionId = divisionId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_DivisionIdIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetDivisionByIdCriteria
            {
                DivisionId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}