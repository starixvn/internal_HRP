namespace HRP.Core.Services
{
    using System.Collections.Generic;
    using Enumerations;
    using Model;

    public interface ISessionService
    {
        int UserId { get; set; }
        int OnUserId { get; set; }
        string Username { get; set; }
        bool IsSystemAdmin { get; set; }
        IEnumerable<MenuItem> MenuItems { get; set; }
        IEnumerable<FunctionModel> Functions { get; set; }

        bool HasPermissionOnFunction(string functionKey, Policy policy);

        bool HasPermissionOnCompany(int companyId, string functionKey, Policy policy);

        void ClearAll();
    }
}