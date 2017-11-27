namespace HRP.Services
{
    using System.Collections.Generic;
    using HRP.Core.Model;

    public class UserService : IUserService
    {
        public UserSession GetUserByUsername(string username)
        {
            return new UserSession
            {
                UserId = 1,
                Username = "admin",
                Password = "1234aa",
                Functions = new List<FunctionModel>
                {
                    new FunctionModel { FunctionId = 1, FunctionKey = "Home", IsHost = false, IsRead = false },
                    new FunctionModel { FunctionId = 1, FunctionKey = "UserManagement", IsHost = true, IsRead = false }
                }
            };
        }
    }
}