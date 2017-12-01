namespace HRP.Services
{
    using System.Collections.Generic;
    using Core.Model;

    public class UserService : IUserService
    {
        public UserSession GetUserByUsername(string username)
        {
            return new UserSession
            {
                UserId = 1,
                Username = "admin",
                Password = "1234aa",
                MenuItems = new List<MenuItem>
                {
                    new MenuItem { MenuId = 1, Name = "Home", Url = "home" },
                    new MenuItem { MenuId = 2, Name = "User Management", Url = "usermanagement" }
                },
                Functions = new List<FunctionModel>
                {
                    new FunctionModel { FunctionId = 1, FunctionKey = "UserManagement", IsHost = true },
                    new FunctionModel { FunctionId = 2, FunctionKey = "WeatherForecasts", IsHost = true }
                }
            };
        }
    }
}