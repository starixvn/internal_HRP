namespace HRP.Services
{
    using HRP.Core.Model;
    
    public interface ITokenService
    {
        string CreateJwtTokens(UserSession user);
    }
}