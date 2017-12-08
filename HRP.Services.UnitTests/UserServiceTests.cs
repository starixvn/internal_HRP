namespace HRP.Services.UnitTest
{
    using HRP.Services;
    using Xunit;

    public class UserServiceTests
    {
        readonly UserService _userService;

        public UserServiceTests()
        {
            _userService = new UserService();
        }

        [Theory]
        [InlineData("admin")]
        public void GetUserByUsername(string username)
        {
            var result = _userService.GetUserByUsername(username);
            Assert.NotNull(result);
        }
    }
}