namespace HRP.Core.Services
{
    using System.Collections.Generic;
    using Model;

    public interface ISessionService
    {
        int UserId { get; set; }
        string Username { get; set; }
        IEnumerable<MenuItem> MenuItems { get; set; }
        int GetUserPermissionOnFunction(int userId, string functionKey);
        void ClearAll();
    }
}