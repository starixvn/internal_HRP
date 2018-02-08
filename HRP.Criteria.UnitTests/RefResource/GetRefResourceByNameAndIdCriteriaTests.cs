namespace HRP.Criteria.UnitTests.RefResource
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using HRP.Criteria.RefResource;
    using Xunit;

    public class GetRefResourceByNameAndIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetRefResourceByNameAndIdCriteria();
            var expected = "RefResource_GetByNameAndId";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_JobGradeIdIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetRefResourceByNameAndIdCriteria
            {
                ResourceId = 1,
                RefName = "ccc"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
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
            var criteria = new GetRefResourceByNameAndIdCriteria
            {
                ResourceId = id,
                RefName = name
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}