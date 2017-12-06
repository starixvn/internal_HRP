namespace HRP.Services
{
    using System;
    using System.Data;
    using Core.Model;
    using HRP.Criteria;
    using Core.Services;
    using HRP.Models;

    public class UserService : BaseService, IUserService
    {
        public User GetUserByUsername(string username)
        {
            GetUserByUsernameCriteria criteria = new GetUserByUsernameCriteria
            {
                Username = username   
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