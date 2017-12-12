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
    }
}