namespace HRP.Core.Services
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Http;
    using Model;

    public class SessionService : ISessionService
    {
        public const string InternalUsername = "InternalUsername";
        public const string InternalUserId = "InternalUserId";
        public const string InternalMenuItemList = "InternalMenuItemList";

        private readonly ISession _session;

        public SessionService(ISession session)
        {
            _session = session;
        }

        public SessionService(IHttpContextAccessor accessor)
        {
            _session = accessor.HttpContext.Session;
        }

        public int UserId
        { 
            get => _session?.GetInt32(InternalUserId) ?? 0;
            set => _session?.SetInt32(InternalUserId, value);
        }

        public string Username
        { 
            get => _session.GetString(InternalUsername) ?? string.Empty;
            set => _session.SetString(InternalUsername, value);
        }

        public IEnumerable<MenuItem> MenuItems 
        { 
            get => _session.Get<List<MenuItem>>(InternalMenuItemList);
            set => _session.Set(InternalMenuItemList, value);
        }

        public int GetUserPermissionOnFunction(int userId, string functionKey)
        {
            return 2;
        }

        public void ClearAll()
        {
            _session.Clear();
        }
    }
}