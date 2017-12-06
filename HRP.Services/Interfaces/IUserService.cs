namespace HRP.Services
{
    using HRP.Core.Model;

    public interface IUserService
    {
        UserSession GetUserByUsername(string username);

        GetUsersResult GetUsers(string keyword, bool? isActive, int pageNumber, int pageSize);
    }
}