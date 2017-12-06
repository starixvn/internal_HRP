namespace HRP.Services
{
    using HRP.Core.Model;
    
    public interface IUserService
    {
        User GetUserByUsername(string username);
    }
}