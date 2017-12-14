namespace HRP.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Services;
    using HRP.Criteria;
    using HRP.Models.UserPermisson;
    using NSubstitute;
    using Xunit;

    public class FunctionServiceTests
    {
        private IBaseService baseService;
        private FunctionService functionService;

        #region GetFunctions

        [Fact]
        public async Task GetTreeFunctions_WhenCalled_ReturnExpectedTreeFunctions()
        {
            // Arrange
            var expected = new List<FunctionModel>
            {
                new FunctionModel { FunctionId = 1 },
                new FunctionModel { FunctionId = 2, ParentId = 1 }
            };
            functionService = GetFunctionService();
            baseService.GetDataAsync<FunctionModel>(Arg.Any<GetAllFunctionCriteria>()).Returns(expected);

            // Act
            var actual = await functionService.GetTreeFunctions(Arg.Any<bool?>());

            // Assert
            Assert.Single(actual);
            Assert.Single(actual.First().Childs);
            Assert.Equal(expected.First(), actual.First());
            Assert.Equal(expected[1], actual.First().Childs.First());
        }

        #endregion GetFunctions

        #region Helpers

        private IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private FunctionService GetFunctionService()
        {
            baseService = FakeBaseService();
            var service = new FunctionService(baseService);

            return service;
        }

        #endregion Helpers
    }
}