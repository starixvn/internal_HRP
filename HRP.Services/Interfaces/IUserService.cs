namespace HRP.Services
{
    using HRP.Core.Model;
    using HRP.Models;

    public interface IUserService
    {
        User GetUserByUsername(string username);
    }
}