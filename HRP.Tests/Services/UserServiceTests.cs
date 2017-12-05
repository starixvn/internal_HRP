namespace HRP.Tests.Services
{
    using HRP.Services;
    using System;
    using Xunit;

    public class UserServiceTests
    {
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userService = new UserService();
        }

        [Fact]
        public void Test1()
        {
            var result = _userService.GetUserByUsername("admin");
            Assert.False(false, "Test unit test");
        }
    }
}