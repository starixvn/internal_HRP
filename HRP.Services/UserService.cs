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
                    new MenuItem { MenuId = 1, Name = "Home" },
                    new MenuItem { MenuId = 2, Name = "User Management" }
                }
            };
        }
    }
}