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
                Password = "1234aa"
            };
        }
    }
}