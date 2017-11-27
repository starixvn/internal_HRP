namespace HRP.Services
{
    using HRP.Core.Model;
    
    public interface IUserService
    {
        UserSession GetUserByUsername(string username);
    }
}