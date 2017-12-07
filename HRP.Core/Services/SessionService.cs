namespace HRP.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Enumerations;
    using Microsoft.AspNetCore.Http;
    using Model;

    public class SessionService : ISessionService
    {
        public const string InternalUsername = "InternalUsername";
        public const string InternalUserId = "InternalUserId";
        public const string InternalMenuItemList = "InternalMenuItemList";
        public const string InternalFunctionList = "InternalFunctionList";

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

        public IEnumerable<FunctionModel> Functions
        {
            get => _session.Get<List<FunctionModel>>(InternalFunctionList);
            set => _session.Set(InternalFunctionList, value);
        }

        public bool HasPermissionOnFunction(string functionKey, Policy policy)
        {
            var functions = Functions.Where(x => x.FunctionKey == functionKey);
            var result = false;

            if (functions != null)
            {
                switch (policy)
                {
                    case Policy.Read:
                        result = functions.Max(x => x.IsRead) || functions.Max(x => x.IsHost);
                        break;

                    case Policy.Host:
                        result = functions.Max(x => x.IsHost);
                        break;

                    default:
                        result = false;
                        break;
                }
            }

            return result;
        }

        public void ClearAll()
        {
            _session.Clear();
        }
    }
}