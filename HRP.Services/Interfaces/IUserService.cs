namespace HRP.Services
{
    using HRP.Core.Model;
    using HRP.Criteria;
    using HRP.Models;

    public interface IUserService
    {
        UserSession GetUserByUsername(string username);

        GetUsersResult GetUsers(string keyword, bool? isActive, int pageNumber, int pageSize);
    }
}