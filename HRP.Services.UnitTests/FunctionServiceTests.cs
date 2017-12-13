namespace HRP.Services.UnitTests
{
    using System.Collections.Generic;
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
        public async Task GetFunctions_WhenCalled_ReturnExpectedFunctions()
        {
            // Arrange
            var expected = new List<FunctionModel>
            {
                new FunctionModel { FunctionId = 1 }
            };
            functionService = GetFunctionService();
            baseService.GetDataAsync<FunctionModel>(Arg.Any<GetAllFunctionCriteria>()).Returns(expected);

            // Act
            var actual = await functionService.GetFunctions(Arg.Any<bool?>());

            // Assert
            Assert.Equal(expected, actual);
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