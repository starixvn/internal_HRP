namespace HRP.Services
{
    using System;
    using System.Data;
    using Core.Model;
    using Fanex.Data;

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

        public GetUsersResult GetUsers(string keyword, bool? isActive, int pageNumber, int pageSize)
        {
            var result = new GetUsersResult();

            using (var objectDb = new ObjectDb("User_GetUsers"))
            {
                var filter = new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Keyword = keyword ?? string.Empty,
                    Total = 0
                };

                objectDb.AddInParameter("IsActive", SqlDbType.Bit, (object)isActive ?? DBNull.Value);
                result.Users = objectDb.Query<UserModel>(filter);
                result.Total = filter.Total;
            }

            return result;
        }
    }
}