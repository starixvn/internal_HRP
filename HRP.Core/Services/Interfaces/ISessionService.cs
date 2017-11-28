namespace HRP.Core.Services
{
    using System.Collections.Generic;
    using Enumerations;
    using Model;

    public interface ISessionService
    {
        int UserId { get; set; }
        string Username { get; set; }
        IEnumerable<MenuItem> MenuItems { get; set; }
        IEnumerable<FunctionModel> Functions { get; set; }
        bool HasPermissionOnFunction(string functionKey, Policy policy);
        void ClearAll();
    }
}